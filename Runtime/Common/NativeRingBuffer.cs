// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

// Ring Buffer implementation inspired by https://www.daugaard.org/blog/writing-a-fast-and-versatile-spsc-ring-buffer/
namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using Unity.Jobs;
    using UnityEngine.XR.MagicLeap.LowLevel.Unsafe;
    using Unsafe;

    namespace LowLevel.Unsafe
    {
        internal unsafe struct UnsafeRingBuffer : IDisposable
        {
            public struct BlockingReader
            {
                [NativeDisableUnsafePtrRestriction]
                private Data* _Data;

                public bool IsCreated => _Data != null;

                internal BlockingReader(void* data)
                {
                    _Data = (Data*)data;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void FinishRead()
                {
                    CheckNullAndThrow(_Data);
                    FinishReadUnchecked();
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void FinishReadUnchecked()
                {
                    _Data->_ReaderShared.StorePosition(ref _Data->_Reader);
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void* PrepareRead(ulong size, ulong alignment)
                {
                    CheckNullAndThrow(_Data);
                    return PrepareReadUnchecked(size, alignment);
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void* PrepareReadUnchecked(ulong size, ulong alignment)
                {
                    var pos = Align(_Data->_Reader.position, alignment);
                    var end = pos + size;
                    if (end > _Data->_Reader.end)
                        GetBufferSpaceToReadFrom(ref pos, ref end);
                    _Data->_Reader.position = end;
                    return _Data->_Reader.buffer + pos;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void* Read(ulong size, ulong alignment)
                {
                    CheckNullAndThrow(_Data);
                    return ReadUnchecked(size, alignment);
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public T Read<T>() where T : unmanaged
                {
                    CheckNullAndThrow(_Data);
                    return *(T*)ReadUnchecked((ulong)sizeof(T), (ulong)UnsafeUtility.AlignOf<T>());
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void* ReadUnchecked(ulong size, ulong alignment)
                    => PrepareReadUnchecked(size, alignment);

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public T ReadUnchecked<T>() where T : unmanaged
                {
                    CheckNullAndThrow(_Data);
                    return *(T*)ReadUnchecked((ulong)sizeof(T), (ulong)UnsafeUtility.AlignOf<T>());
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void* ReadArray(ulong elementSize, ulong elementAlignment, ulong count)
                {
                    CheckNullAndThrow(_Data);
                    return ReadArrayUnchecked(elementSize, elementAlignment, count);
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void* ReadArrayUnchecked(ulong elementSize, ulong elementAlignment, ulong count)
                    => PrepareReadUnchecked(elementSize * count, elementAlignment);

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                private void GetBufferSpaceToReadFrom(ref ulong position, ref ulong end)
                {
                    if (end > _Data->_Reader.size)
                    {
                        end -= position;
                        position = 0;
                        _Data->_Reader.@base += _Data->_Reader.size;
                    }
                    for (;;)
                    {
                        var writerPos = _Data->_WriterShared.LoadPosition();
                        var available = writerPos - _Data->_Reader.@base;
                        // Signed comparison (available can be negative)
                        if ((long)available >= (long)end)
                        {
                            _Data->_Reader.end = Math.Min(available, _Data->_Reader.size);
                            break;
                        }
                    }
                }
            }

            public struct BlockingWriter
            {
                [NativeDisableUnsafePtrRestriction]
                private Data* _Data;

                public bool IsCreated => _Data != null;

                internal BlockingWriter(void* data)
                {
                    _Data = (Data*)data;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void FinishWrite()
                {
                    CheckNullAndThrow(_Data);
                    FinishWriteUnchecked();
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void FinishWriteUnchecked()
                {
                    _Data->_WriterShared.StorePosition(ref _Data->_Writer);
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void* PrepareWrite(ulong size, ulong alignment)
                {
                    CheckNullAndThrow(_Data);
                    return PrepareWriteUnchecked(size, alignment);
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void* PrepareWriteUnchecked(ulong size, ulong alignment)
                {
                    var pos = Align(_Data->_Writer.position, alignment);
                    var end = pos + size;
                    if (end > _Data->_Writer.end)
                        GetBufferSpaceToWriteTo(ref pos, ref end);
                    _Data->_Writer.position = end;
                    return _Data->_Writer.buffer + pos;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void Write(void* value, ulong size, ulong alignment)
                {
                    CheckNullAndThrow(_Data);
                    WriteUnchecked(value, size, alignment);
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void Write<T>(T* value) where T : unmanaged
                    => Write(value, (ulong)sizeof(T), (ulong)UnsafeUtility.AlignOf<T>());

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void Write<T>(ref T value) where T : unmanaged
                    => Write(UnsafeUtility.AddressOf(ref value), (ulong)sizeof(T), (ulong)UnsafeUtility.AlignOf<T>());

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void WriteUnchecked(void* value, ulong size, ulong alignment)
                {
                    var dest = PrepareWriteUnchecked(size, alignment);
                    UnsafeUtility.MemCpy(dest, value, (long)size);
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void WriteArray(void* values, ulong elementSize, ulong elementAlignment, ulong count)
                {
                    CheckNullAndThrow(_Data);
                    WriteArrayUnchecked(values, elementSize, elementAlignment, count);
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void WriteArrayUnchecked(void* values, ulong elementSize, ulong elementAlignment, ulong count)
                {
                    var szInBytes = elementSize * count;
                    var dest = PrepareWriteUnchecked(szInBytes, elementAlignment);
                    UnsafeUtility.MemCpy(dest, values, (long)szInBytes);
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void WriteArray<T>(T* values, ulong count) where T : unmanaged
                    => WriteArray(values, (ulong)sizeof(T), (ulong)UnsafeUtility.AlignOf<T>(), count);

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                private void GetBufferSpaceToWriteTo(ref ulong position, ref ulong end)
                {
                    if (end > _Data->_Writer.size)
                    {
                        end -= position;
                        position = 0;
                        _Data->_Writer.@base += _Data->_Writer.size;
                    }
                    for (;;)
                    {
                        var readerPos = _Data->_ReaderShared.LoadPosition();
                        var available = readerPos - _Data->_Writer.@base + _Data->_Writer.size;
                        // Signed comparison (available can be negative)
                        if ((long)available >= (long)end)
                        {
                            _Data->_Writer.end = Math.Min(available, _Data->_Writer.size);
                            break;
                        }
                    }
                }
            }

            private struct Data
            {
                public LocalState _Reader;
                public SharedState _ReaderShared;
                public LocalState _Writer;
                public SharedState _WriterShared;
                public Allocator Allocator;
            }

            // Using the StructLayout attribute to force the struct to be cache-aligned, to avoid false-sharing.
            [StructLayout(LayoutKind.Sequential, Size = 64)]
            private struct LocalState
            {
                [NativeDisableUnsafePtrRestriction]
                public byte* buffer;
                public ulong position;
                public ulong end;
                public ulong @base;
                public ulong size;
            }

            public struct NonblockingReader
            {
                [NativeDisableUnsafePtrRestriction]
                private Data* _Data;

                public bool IsCreated => _Data != null;

                internal NonblockingReader(void* data)
                {
                    _Data = (Data*)data;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void FinishRead()
                {
                    CheckNullAndThrow(_Data);
                    FinishReadUnchecked();
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void FinishReadUnchecked()
                {
                    _Data->_ReaderShared.StorePosition(ref _Data->_Reader);
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public bool TryPrepareRead(ulong size, ulong alignment, out void* outData)
                {
                    CheckNullAndThrow(_Data);
                    return TryPrepareReadUnchecked(size, alignment, out outData);
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public bool TryPrepareReadUnchecked(ulong size, ulong alignment, out void* outData)
                {
                    outData = null;
                    var pos = Align(_Data->_Reader.position, alignment);
                    var end = pos + size;
                    var success = end <= _Data->_Reader.end || TryGetBufferSpaceToReadFromNonBlocking(ref pos, ref end);
                    if (!success)
                        return false;

                    _Data->_Reader.position = end;
                    outData = _Data->_Reader.buffer + pos;
                    return true;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public bool TryRead(ulong size, ulong alignment, out void* outData)
                {
                    CheckNullAndThrow(_Data);
                    return TryPrepareReadUnchecked(size, alignment, out outData);
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public bool TryRead<T>(out T outData) where T : unmanaged
                {
                    CheckNullAndThrow(_Data);
                    outData = default;
                    var ret = TryPrepareReadUnchecked((ulong)sizeof(T), (ulong)UnsafeUtility.AlignOf<T>(), out var ptr);
                    if (!ret)
                        return false;
                    outData = *(T*)ptr;
                    return true;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public bool TryReadArray(ulong elementSize, ulong elementAlignment, ulong elementCount, out void* outData)
                {
                    CheckNullAndThrow(_Data);
                    return TryPrepareReadUnchecked(elementSize * elementCount, elementAlignment, out outData);
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public bool TryReadArray<T>(ulong elementCount, Allocator allocator, out NativeArray<T> outData) where T : unmanaged
                {
                    CheckNullAndThrow(_Data);
                    outData = default;
                    var sizeInBytes = (ulong)sizeof(T) * elementCount;
                    var ret = TryPrepareReadUnchecked(sizeInBytes, (ulong)UnsafeUtility.AlignOf<T>(), out var ptr);
                    if (!ret)
                        return false;
                    outData = new NativeArray<T>((int)elementCount, allocator, NativeArrayOptions.UninitializedMemory);
                    UnsafeUtility.MemCpy(outData.GetUnsafePtr(), ptr, (long)sizeInBytes);
                    return true;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                private bool TryGetBufferSpaceToReadFromNonBlocking(ref ulong position, ref ulong end)
                {
                    ulong @base = _Data->_Reader.@base;
                    if (end > _Data->_Reader.size)
                    {
                        end -= position;
                        position = 0;
                        @base += _Data->_Reader.size;
                    }
                    var writerPos = _Data->_WriterShared.LoadPosition();
                    var available = writerPos - @base;

                    // Signed comparison (available can be negative)
                    var success = (long)available >= (long)end;
                    if (!success)
                        return false;

                    _Data->_Reader.@base = @base;
                    _Data->_Reader.end = Math.Min(available, _Data->_Reader.size);
                    return true;
                }
            }

            public struct NonblockingWriter
            {
                [NativeDisableUnsafePtrRestriction]
                private Data* _Data;

                public bool IsCreated => _Data != null;

                internal NonblockingWriter(void* data)
                {
                    _Data = (Data*)data;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void FinishWrite()
                {
                    CheckNullAndThrow(_Data);
                    FinishWriteUnchecked();
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void FinishWriteUnchecked()
                {
                    _Data->_WriterShared.StorePosition(ref _Data->_Writer);
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public bool TryPrepareWrite(ulong size, ulong alignment, out void* outData)
                {
                    CheckNullAndThrow(_Data);
                    return TryPrepareWriteUnchecked(size, alignment, out outData);
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public bool TryPrepareWriteUnchecked(ulong size, ulong alignment, out void* outData)
                {
                    outData = null;
                    var pos = Align(_Data->_Writer.position, alignment);
                    var end = pos + size;
                    var success = end <= _Data->_Writer.end || TryGetBufferSpaceToWriteToNonBlocking(ref pos, ref end);
                    if (!success)
                        return false;

                    _Data->_Writer.position = end;
                    outData = _Data->_Writer.buffer + pos;
                    return true;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public bool TryWrite(void* value, ulong size, ulong alignment)
                {
                    CheckNullAndThrow(_Data);
                    return TryWriteUnchecked(value, size, alignment);
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public bool TryWrite<T>(T value) where T : unmanaged
                {
                    CheckNullAndThrow(_Data);
                    return TryWriteUnchecked(&value, (ulong)sizeof(T), (ulong)UnsafeUtility.AlignOf<T>());
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public bool TryWriteArray(void* values, ulong elementSize, ulong elementAlignment, ulong elementCount)
                {
                    CheckNullAndThrow(_Data);
                    return TryWriteUnchecked(values, elementSize * elementCount, elementAlignment);
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public bool TryWriteArray<T>(NativeArray<T> array) where T : unmanaged
                {
                    CheckNullAndThrow(_Data);
                    var sizeInBytes = sizeof(T) * array.Length;
                    if (!TryPrepareWriteUnchecked((ulong)sizeInBytes, (ulong)UnsafeUtility.AlignOf<T>(), out var ptr))
                        return false;
                    UnsafeUtility.MemCpy(ptr, array.GetUnsafeReadOnlyPtr(), sizeInBytes);
                    return true;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public bool TryWriteUnchecked(void* value, ulong size, ulong alignment)
                {
                    if (!TryPrepareWriteUnchecked(size, alignment, out var ptr))
                        return false;
                    UnsafeUtility.MemCpy(ptr, value, (long)size);
                    return true;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                private bool TryGetBufferSpaceToWriteToNonBlocking(ref ulong position, ref ulong end)
                {
                    ulong @base = _Data->_Writer.@base;
                    if (end > _Data->_Writer.size)
                    {
                        end -= position;
                        position = 0;
                        @base += _Data->_Writer.size;
                    }
                    var readerPos = _Data->_ReaderShared.LoadPosition();
                    var available = readerPos - @base + _Data->_Writer.size;

                    // Signed comparison (available can be negative)
                    var success = (long)available >= (long)end;
                    if (!success)
                        return false;

                    _Data->_Writer.@base = @base;
                    _Data->_Writer.end = Math.Min(available, _Data->_Writer.size);
                    return true;

                }
            }

            [StructLayout(LayoutKind.Sequential, Size = 64)]
            private struct SharedState
            {
                private ulong position;

                // in .NET, 64-bit reads and writes are atomic on 64-bit systems.
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public ulong LoadPosition()
                    => position;

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public void StorePosition(ref LocalState localState)
                    => position = localState.@base + localState.position;
            }

            [NativeDisableUnsafePtrRestriction]
            private Data* _Data;

            public Allocator Allocator => IsCreated ? _Data->Allocator : Allocator.Invalid;

            public bool IsCreated => _Data != null;

            public UnsafeRingBuffer(long size, Allocator allocator)
            {
                CheckBufferParametersAndThrow(size, allocator);
                _Data = UnsafeUtilityEx.CallocTracked<Data>(allocator);
                _Data->Allocator = allocator;
                InitializeUnchecked(size, allocator);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public BlockingReader AsBlockingReader()
            {
                CheckNullAndThrow(_Data);
                return AsBlockingReaderUnchecked();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public BlockingReader AsBlockingReaderUnchecked()
                => new BlockingReader(_Data);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public BlockingWriter AsBlockingWriter()
            {
                CheckNullAndThrow(_Data);
                return AsBlockingWriterUnchecked();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public BlockingWriter AsBlockingWriterUnchecked()
                => new BlockingWriter(_Data);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public NonblockingReader AsNonblockingReader()
            {
                CheckNullAndThrow(_Data);
                return AsNonblockingReaderUnchecked();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public NonblockingReader AsNonblockingReaderUnchecked()
                => new NonblockingReader(_Data);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public NonblockingWriter AsNonblockingWriter()
            {
                CheckNullAndThrow(_Data);
                return AsNonblockingWriterUnchecked();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public NonblockingWriter AsNonblockingWriterUnchecked()
                => new NonblockingWriter(_Data);

            public void Dispose()
            {
                CheckDisposedAndThrow(_Data);

                Reset();
                var allocator = _Data->Allocator;
                UnsafeUtility.FreeTracked(_Data, allocator);
                _Data = null;
            }

            public void Initialize(long size, Allocator allocator)
            {
                CheckNullAndThrow(_Data);
                CheckBufferParametersAndThrow(size, allocator);
                InitializeUnchecked(size, allocator);
            }

            private void InitializeUnchecked(long size, Allocator allocator)
            {
                ResetUnchecked();
                var buffer = UnsafeUtility.MallocTracked(size, UnsafeUtility.AlignOf<byte>(), allocator, 0);
                _Data->_Reader.buffer = _Data->_Writer.buffer = (byte*)buffer;
                _Data->_Reader.size = _Data->_Writer.size = (ulong)size;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Reset()
            {
                CheckNullAndThrow(_Data);
                ResetUnchecked();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void ResetUnchecked()
            {
                if (_Data->_Reader.buffer != null)
                    UnsafeUtility.FreeTracked(_Data->_Reader.buffer, _Data->Allocator);
                _Data->_Reader = _Data->_Writer = new LocalState();
                _Data->_ReaderShared = _Data->_WriterShared = new SharedState();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static ulong Align(ulong pos, ulong alignment)
                => (pos + alignment - 1) & ~(alignment - 1);

            [Conditional("DEVELOPMENT_BUILD"), Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
            private static void CheckBufferParametersAndThrow(long size, Allocator allocator)
            {
                if (!IsPowerofTwo(size))
                    throw new ArgumentException($"{nameof(size)} is not a power of two");
                if (allocator < Allocator.Temp)
                    throw new ArgumentException($"{nameof(allocator)} is not a valid allocator");
            }

            [Conditional("DEVELOPMENT_BUILD"), Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
            private static void CheckDisposedAndThrow(Data* data)
            {
                if (data == null)
                    throw new ObjectDisposedException("RingBuffer has already been disposed");
            }

            [Conditional("DEVELOPMENT_BUILD"), Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
            private static void CheckNullAndThrow(Data* data)
            {
                if (data == null)
                    throw new NullReferenceException("RingBuffer is not properly initialized");
            }

            // Function to check if x is power of 2
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static bool IsPowerofTwo(long n)
            {
                if (n == 0)
                    return false;
                if ((n & (~(n - 1))) == n)
                    return true;
                return false;
            }
        }
    }

    internal unsafe struct NativeRingBuffer : IDisposable
    {
        [NativeContainer]
        public struct BlockingReader : IDisposable
        {
            private UnsafeRingBuffer.BlockingReader m_Reader;
            [NativeDisableUnsafePtrRestriction]
            private int* m_Count;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            private AtomicSafetyHandle m_Safety;
#endif

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            internal BlockingReader(UnsafeRingBuffer.BlockingReader reader, int* readerCount, AtomicSafetyHandle safety)
#else
            internal BlockingReader(UnsafeRingBuffer.BlockingReader reader, int* readerCount)
#endif
            {
                m_Reader = reader;
                m_Count = readerCount;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                m_Safety = safety;
#endif
            }

            public void Dispose()
            {
                CheckSafetyHandle();

                if (m_Count == null)
                    return;
                Interlocked.Decrement(ref UnsafeUtility.AsRef<int>(m_Count));
            }

            public JobHandle Dispose(JobHandle depends)
            {
                CheckSafetyHandle();

                return new DecrementCountJob { Count = m_Count }.Schedule(depends);
            }

            public void FinishRead()
            {
                CheckSafetyHandle();

                m_Reader.FinishRead();
            }

            public T Read<T>() where T : unmanaged
            {
                CheckSafetyHandle();

                return m_Reader.Read<T>();
            }

            [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
            private void CheckSafetyHandle()
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (!AtomicSafetyHandle.IsHandleValid(m_Safety))
                    throw new ObjectDisposedException("This container has already been disposed");
#endif
            }
        }

        [NativeContainer]
        public struct BlockingWriter : IDisposable
        {
            private UnsafeRingBuffer.BlockingWriter m_Writer;
            [NativeDisableUnsafePtrRestriction]
            private int* m_Count;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            private AtomicSafetyHandle m_Safety;
#endif

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            internal BlockingWriter(UnsafeRingBuffer.BlockingWriter writer, int* writerCount, AtomicSafetyHandle safety)
#else
            internal BlockingWriter(UnsafeRingBuffer.BlockingWriter writer, int* writerCount)
#endif
            {
                m_Writer = writer;
                m_Count = writerCount;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                m_Safety = safety;
#endif
            }

            public void Dispose()
            {
                CheckSafetyHandle();

                if (m_Count == null)
                    return;
                Interlocked.Decrement(ref UnsafeUtility.AsRef<int>(m_Count));
            }

            public JobHandle Dispose(JobHandle depends)
            {
                CheckSafetyHandle();

                return new DecrementCountJob { Count = m_Count }.Schedule(depends);
            }

            public void FinishWrite()
            {
                CheckSafetyHandle();

                m_Writer.FinishWrite();
            }

            public void Write<T>(T value) where T : unmanaged
            {
                CheckSafetyHandle();

                m_Writer.Write(&value);
            }

            public void WriteRef<T>(ref T value) where T : unmanaged
            {
                CheckSafetyHandle();

                m_Writer.Write(ref value);
            }

            [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
            private void CheckSafetyHandle()
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (!AtomicSafetyHandle.IsHandleValid(m_Safety))
                    throw new ObjectDisposedException("This container has already been disposed");
#endif
            }
        }

        [NativeContainer]
        public struct NonblockingReader : IDisposable
        {
            private UnsafeRingBuffer.NonblockingReader m_Reader;
            [NativeDisableUnsafePtrRestriction]
            private int* m_Count;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            private AtomicSafetyHandle m_Safety;
#endif

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            internal NonblockingReader(UnsafeRingBuffer.NonblockingReader reader, int* readerCount,
                AtomicSafetyHandle safety)
#else
            internal NonblockingReader(UnsafeRingBuffer.NonblockingReader reader, int* readerCount)
#endif
            {
                m_Reader = reader;
                m_Count = readerCount;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                m_Safety = safety;
#endif
            }

            public void Dispose()
            {
                CheckSafetyHandle();

                if (m_Count == null)
                    return;
                Interlocked.Decrement(ref UnsafeUtility.AsRef<int>(m_Count));
            }

            public JobHandle Dispose(JobHandle depends)
            {
                CheckSafetyHandle();

                return new DecrementCountJob { Count = m_Count }.Schedule(depends);
            }

            public void FinishRead()
            {
                CheckSafetyHandle();

                m_Reader.FinishRead();
            }

            public bool TryRead<T>(out T value) where T : unmanaged
            {
                CheckSafetyHandle();

                return m_Reader.TryRead(out value);
            }

            [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
            private void CheckSafetyHandle()
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (!AtomicSafetyHandle.IsHandleValid(m_Safety))
                    throw new ObjectDisposedException("This container has already been disposed");
#endif
            }
        }

        [NativeContainer]
        public struct NonblockingWriter : IDisposable
        {
            private UnsafeRingBuffer.NonblockingWriter m_Writer;
            [NativeDisableUnsafePtrRestriction]
            private int* m_Count;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            private AtomicSafetyHandle m_Safety;
#endif

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            internal NonblockingWriter(UnsafeRingBuffer.NonblockingWriter writer, int* writerCount,
                AtomicSafetyHandle safety)
#else
            internal NonblockingWriter(UnsafeRingBuffer.NonblockingWriter writer, int* writerCount)
#endif
            {
                m_Writer = writer;
                m_Count = writerCount;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                m_Safety = safety;
#endif
            }

            public void Dispose()
            {
                CheckSafetyHandle();

                if (m_Count == null)
                    return;
                Interlocked.Decrement(ref UnsafeUtility.AsRef<int>(m_Count));
            }

            public JobHandle Dispose(JobHandle depends)
            {
                CheckSafetyHandle();

                return new DecrementCountJob { Count = m_Count }.Schedule(depends);
            }

            public void FinishWrite()
            {
                CheckSafetyHandle();

                m_Writer.FinishWrite();
            }

            public bool TryWrite<T>(T value) where T : unmanaged
            {
                CheckSafetyHandle();

                return m_Writer.TryWrite(value);
            }

            [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
            private void CheckSafetyHandle()
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                if (!AtomicSafetyHandle.IsHandleValid(m_Safety))
                    throw new ObjectDisposedException("This container has already been disposed");
#endif
            }
        }

        private UnsafeRingBuffer m_RingBuffer;
        private int* m_ReaderCount;
        private int* m_WriterCount;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        private AtomicSafetyHandle m_Safety;

        private static int s_StaticSafetyId = AtomicSafetyHandle.NewStaticSafetyId<NativeRingBuffer>();
#endif

        public bool IsCreated => m_RingBuffer.IsCreated;

        public NativeRingBuffer(long size, Allocator allocator)
        {
            m_RingBuffer = new UnsafeRingBuffer(size, allocator);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            m_Safety = UnsafeUtilityEx.CreateAtomicSafetyHandleForAllocator(allocator);
            AtomicSafetyHandle.SetStaticSafetyId(ref m_Safety, s_StaticSafetyId);
#endif
            m_ReaderCount = UnsafeUtilityEx.CallocTracked<int>(allocator);
            m_WriterCount = UnsafeUtilityEx.CallocTracked<int>(allocator);
        }

        private struct DisposeData
        {
            public UnsafeRingBuffer m_RingBuffer;
            public int* m_ReaderCount;
            public int* m_WriterCount;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            public AtomicSafetyHandle m_Safety;
#endif

            public void Dispose()
            {
                if (m_ReaderCount == null || *m_ReaderCount != 0)
                    throw new InvalidOperationException("Cannot dispose buffer with active readers");

                UnsafeUtility.FreeTracked(m_ReaderCount, m_RingBuffer.Allocator);

                if (m_WriterCount == null || *m_WriterCount != 0)
                    throw new InvalidOperationException("Cannot dispose buffer with active writers");

                UnsafeUtility.FreeTracked(m_WriterCount, m_RingBuffer.Allocator);

                m_RingBuffer.Dispose();
            }
        }

        public void Dispose()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckDeallocateAndThrow(m_Safety);
            AtomicSafetyHandle.Release(m_Safety);

            new DisposeData{ m_RingBuffer = m_RingBuffer, m_Safety = m_Safety, m_ReaderCount = m_ReaderCount, m_WriterCount = m_WriterCount }.Dispose();
#else
            new DisposeData{ m_RingBuffer = m_RingBuffer, m_ReaderCount = m_ReaderCount, m_WriterCount = m_WriterCount }.Dispose();
#endif
            m_RingBuffer = default;
            m_ReaderCount = null;
            m_WriterCount = null;
        }

        private struct DecrementCountJob : IJob
        {
            [NativeDisableUnsafePtrRestriction] public int* Count;

            public void Execute()
            {
                if (Count == null)
                    return;
                Interlocked.Decrement(ref UnsafeUtility.AsRef<int>(Count));
            }
        }

        private struct DisposeJob : IJob
        {
            public DisposeData data;

            public void Execute()
            {
                data.Dispose();
            }
        }

        public JobHandle Dispose(JobHandle depends)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckDeallocateAndThrow(m_Safety);
#endif

            var handle = new DisposeJob
            {
                data = new DisposeData
                {
                    m_RingBuffer = m_RingBuffer,
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                    m_Safety = m_Safety,
#endif
                    m_ReaderCount = m_ReaderCount,
                    m_WriterCount = m_WriterCount
                }
            }.Schedule(depends);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.Release(m_Safety);
#endif
            m_RingBuffer = default;
            m_ReaderCount = null;
            m_WriterCount = null;
            return handle;
        }

        public BlockingReader AsBlockingReader()
        {
            if (!IsCreated)
                throw new NullReferenceException();

            CheckIfOnlyReaderAndThrow();

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            return new BlockingReader(m_RingBuffer.AsBlockingReader(), m_ReaderCount, m_Safety);
#else
            return new BlockingReader(m_RingBuffer.AsBlockingReader(), m_ReaderCount);
#endif

        }

        public BlockingWriter AsBlockingWriter()
        {
            if (!IsCreated)
                throw new NullReferenceException();

            CheckIfOnlyWriterAndThrow();

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            return new BlockingWriter(m_RingBuffer.AsBlockingWriter(), m_WriterCount, m_Safety);
#else
            return new BlockingWriter(m_RingBuffer.AsBlockingWriter(), m_WriterCount);
#endif
        }

        public NonblockingReader AsNonblockingReader()
        {
            if (!IsCreated)
                throw new NullReferenceException();

            CheckIfOnlyReaderAndThrow();

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            return new NonblockingReader(m_RingBuffer.AsNonblockingReader(), m_ReaderCount, m_Safety);
#else
            return new NonblockingReader(m_RingBuffer.AsNonblockingReader(), m_ReaderCount);
#endif
        }

        public NonblockingWriter AsNonblockingWriter()
        {
            if (!IsCreated)
                throw new NullReferenceException();

            CheckIfOnlyWriterAndThrow();

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            return new NonblockingWriter(m_RingBuffer.AsNonblockingWriter(), m_WriterCount, m_Safety);
#else
            return new NonblockingWriter(m_RingBuffer.AsNonblockingWriter(), m_WriterCount);
#endif
        }

        private void CheckIfOnlyReaderAndThrow()
        {
            if (Interlocked.CompareExchange(ref UnsafeUtility.AsRef<int>(m_ReaderCount), 1, 0) != 0)
                throw new InvalidOperationException("only a single reader instance may exist at any time");
        }

        private void CheckIfOnlyWriterAndThrow()
        {
            if (Interlocked.CompareExchange(ref UnsafeUtility.AsRef<int>(m_WriterCount), 1, 0) != 0)
                throw new InvalidOperationException("only a single writer instance may exist at any time");
        }
    }
}
