using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine.PlayerLoop;
using UnityEngine.XR.MagicLeap.Unsafe;

namespace UnityEngine.XR.MagicLeap
{
    [NativeContainer]
    internal unsafe struct NativeSyncBuffer<T> : IDisposable where T : unmanaged
    {
        private struct BufferData
        {
            public T input;
            public T output;
        }

        private struct DisposeData
        {
            [NativeDisableUnsafePtrRestriction]
            public BufferData* m_Data;

            public Allocator m_Allocator;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            public AtomicSafetyHandle m_Safety;
#endif
        }

        private struct DisposeJob : IJob
        {
            public DisposeData Data;

            public void Execute()
            {
                if (Data.m_Data == null || Data.m_Allocator <= Allocator.None)
                    return;
                
                UnsafeUtility.FreeTracked(Data.m_Data, Data.m_Allocator);
            }
        }

        private struct SyncJob : IJob
        {
            public NativeSyncBuffer<T> Buffer;

            public void Execute() => Buffer.Sync();
        }

        private struct UpdateJob : IJob
        {
            public NativeSyncBuffer<T> Buffer;
            public T m_Data;

            public void Execute() => Buffer.UpdateInput(m_Data);
        }

        [NativeDisableUnsafePtrRestriction]
        private BufferData* m_BufferData;
        private Allocator m_Allocator;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        internal AtomicSafetyHandle m_Safety;

        internal static readonly int s_staticSafetyId = AtomicSafetyHandle.NewStaticSafetyId<NativeSyncBuffer<T>>();
#endif

        public bool IsValid => m_BufferData != null && m_Allocator != Allocator.Invalid;

        public T Input
        {
            set => UpdateInput(value);
        }

        public T Output => *GetOutputPointerReadOnly();

        public NativeSyncBuffer(Allocator allocator, T initialState = default)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (!UnsafeUtility.IsUnmanaged<T>())
                throw new Exception(
                    $"{typeof(T).Name} is a managed type and is not a valid type for use in a NativeSyncBuffer container!");
            
            m_Safety = AtomicSafetyHandle.Create();
            AtomicSafetyHandle.SetStaticSafetyId(ref m_Safety, s_staticSafetyId);
            AtomicSafetyHandle.SetNestedContainer(m_Safety, UnsafeUtility.IsNativeContainerType<T>());
#endif
            m_BufferData = UnsafeUtilityEx.MallocTracked<BufferData>(allocator, 1);
            *m_BufferData = new BufferData()
            {
                input = initialState,
                output = initialState,
            };
            m_Allocator = allocator;
        }

        public void Dispose()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckDeallocateAndThrow(m_Safety);
            AtomicSafetyHandle.Release(m_Safety);
#endif
            
            if (m_BufferData == null || m_Allocator <= Allocator.None)
                return;
            
            UnsafeUtility.FreeTracked(m_BufferData, m_Allocator);
            m_BufferData = null;
            m_Allocator = Allocator.Invalid;
        }

        // Having a job-ified Dispose() allows containers to be trivially chained
        // to the end of a job dependency chain, and thus no longer require explicit
        // cleanup by users.
        public JobHandle Dispose(JobHandle deps)
        {
            // Unlike the synchronous Dispose(), we don't need
            // to call CheckDeallocateAndThrow() here, because
            // we're already waiting on any existing jobs to finish
            // via the deps JobHandle.
            if (m_BufferData == null || m_Allocator <= Allocator.None)
                return deps;

            var job = new DisposeJob()
            {
                Data = new DisposeData()
                {
                    m_Data = m_BufferData,
                    m_Allocator = m_Allocator,
                    // We include the AtomicSafetyHandle here
                    // so the Job System will catch any
                    // jobs that reference this
                    // container that weren't
                    // properly included in the
                    // deps JobHandle.
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                    m_Safety = m_Safety,
#endif
                }
            };
            
            var handle = job.Schedule(deps);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            // It's safe to call Release here, because
            // the preceding Schedule() call causes
            // the Job System to hold a reference
            // to the Safety Handle until the job
            // is complete.
            AtomicSafetyHandle.Release(m_Safety);
#endif

            m_BufferData = null;
            m_Allocator = Allocator.Invalid;

            return handle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T* GetInputPointer()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif
            return GetInputPointerUnchecked();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T* GetInputPointerUnchecked()
            => (T*)UnsafeUtility.AddressOf(ref m_BufferData->input);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T* GetOutputPointerReadOnly()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
            return GetOutputPointerUnchecked();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T* GetOutputPointerUnchecked()
            => (T*)UnsafeUtility.AddressOf(ref m_BufferData->output);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Sync()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif
            m_BufferData->output = m_BufferData->input;
        }

        public JobHandle Sync(JobHandle deps)
            => new SyncJob { Buffer = this }.Schedule(deps);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateInput(in T data)
            => *GetInputPointer() = data;

        public JobHandle UpdateInputAsync(in T data, JobHandle deps)
            => new UpdateJob { Buffer = this, m_Data = data }.Schedule(deps);
    }
}
