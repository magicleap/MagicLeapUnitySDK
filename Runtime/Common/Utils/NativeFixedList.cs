#if UNITY_XR_MAGICLEAP_PROVIDER
// This class also exists inside Unity's Magic Leap XR Plugin so we want to avoid naming conflicts.
#else
using System;
using Unity.Collections;

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// Has List-like semantics (Capacity and Length) using a NativeArray as the backing store.
    /// The NativeArray is never resized. This is useful for times you don't know how big the
    /// array will be, but there is a definite upper bound.
    /// This list supports duck-typed foreach Enumerator semantics.
    /// </summary>
    internal struct NativeFixedList<T> : IEquatable<NativeFixedList<T>>, IDisposable where T : struct
    {
        /// <summary>
        /// Allocates a new NativeFixedList with <paramref name="Capacity"/>.
        /// Caller must Dispose the array when no longer needed.
        /// </summary>
        /// <param name="Capacity"></param>
        /// <param name="allocator"></param>
        public NativeFixedList(int Capacity, Allocator allocator)
        {
            m_NativeArray = new NativeArray<T>(Capacity, allocator);
            Length = 0;
        }

        /// <summary>
        /// Create a NativeFixedList from an existing NativeArray and a length.
        /// This NativeFixedList now owns the memory and should be Disposed when finished.
        /// </summary>
        /// <param name="other">The array to take ownership of</param>
        /// <param name="length">The number of elements actually used by the NativeArray</param>
        public NativeFixedList(NativeArray<T> other, int length)
        {
            m_NativeArray = other;
            Length = length;
        }

        public bool IsCreated
        {
            get { return m_NativeArray.IsCreated; }
        }

        public int Capacity
        {
            get { return m_NativeArray.Length; }
        }

        public int Length { get; private set; }

        public T this[int index]
        {
            get
            {
                return m_NativeArray[index];
            }
            set
            {
                m_NativeArray[index] = value;
            }
        }

        public void Clear()
        {
            Length = 0;
        }

        public void Add(T item)
        {
            if (Length == Capacity)
                throw new InvalidOperationException($"Cannot Add when Length ({Length}) is already at ({Capacity})");

            m_NativeArray[Length++] = item;
        }

        /// <summary>
        /// Copies the contents of this list to another NativeArray.
        /// <paramref name="destination"/> must have the same Length as this list.
        /// </summary>
        /// <param name="destination">The destination array</param>
        public void CopyTo(NativeArray<T> destination)
        {
            NativeArray<T>.Copy(m_NativeArray, destination, Length);
        }

        public void Dispose()
        {
            m_NativeArray.Dispose();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = m_NativeArray.GetHashCode();
                hash = hash * 486187739 + Length.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            return ((obj is NativeFixedList<T>) && Equals((NativeFixedList<T>)obj));
        }

        public bool Equals(NativeFixedList<T> other)
        {
            return
                m_NativeArray.Equals(other.m_NativeArray) &&
                (Length == other.Length);
        }

        public static bool operator ==(NativeFixedList<T> lhs, NativeFixedList<T> rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(NativeFixedList<T> lhs, NativeFixedList<T> rhs)
        {
            return !lhs.Equals(rhs);
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public struct Enumerator
        {
            internal Enumerator(NativeFixedList<T> list)
            {
                m_Index = -1;
                m_NativeFixedList = list;
            }

            public bool MoveNext()
            {
                return ++m_Index < m_NativeFixedList.Length;
            }

            public void Reset()
            {
                m_Index = -1;
            }

            public T Current
            {
                get
                {
                    return m_NativeFixedList[m_Index];
                }
            }

            public void Dispose()
            {
                m_NativeFixedList = default(NativeFixedList<T>);
                m_Index = 0;
            }

            int m_Index;

            NativeFixedList<T> m_NativeFixedList;
        }

        NativeArray<T> m_NativeArray;
    }
}
#endif
