// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "CircularBuffer.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Collections;

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// This is a specialized utility class meant to be used within the Unity SDK
    /// for very specific cases. This should not be taken as a general purpose
    /// Circular buffer as it does not cater to all possible use cases.
    /// </summary>
    /// <typeparam name="T">Type of data to store in the buffer</typeparam>
    internal class CircularBuffer<T> : IEnumerator, IEnumerable
    {
        private T[] buffer = null;
        private int bufferIndex = 0;

        private int front;
        private int back;

        private int enumeratorIndex;

        /// <summary>
        /// Get the current value when enumerating the buffer
        /// </summary>
        public object Current => buffer[enumeratorIndex];

        /// <summary>
        /// Max number of elements the buffer can hold.
        /// </summary>
        public int Capacity => buffer.Length;

        /// <summary>
        /// Number of elements stored in the buffer
        /// </summary>
        public int Count
        {
            get
            {
                if (front == -1)
                {
                    return 0;
                }

                return (front <= back) ? back - front + 1 : ((buffer.Length - front) + (back + 1));
            }
        }

        /// <summary>
        /// Create a CircularBuffer with preinitialized objects.
        /// When creating the buffer with this factory method, use the Get() method to get
        /// a reference to the next available object.
        /// DO NOT use any other methods available in this class as they;re relevant only
        /// when the CircularBuffer is instantiated with a fixed size and no preinitialized
        /// objects.
        /// </summary>
        /// <param name="objects">Array of objects to initialize the Circular Buffer with.</param>
        /// <returns></returns>
        public static CircularBuffer<T> Create(params T[] objects)
        {
            CircularBuffer<T> circularBuffer = new CircularBuffer<T>(objects);
            return circularBuffer;
        }

        /// <summary>
        /// Create a fixed size buffer initialized with the defined objects.
        /// DO NOT use any other methods available in this class as they;re relevant only
        /// when the CircularBuffer is instantiated with a fixed size and no preinitialized
        /// objects.
        /// </summary>
        /// <param name="objects">Default values of the buffer</param>
        private CircularBuffer(params T[] objects)
        {
            this.buffer = objects;
        }

        /// <summary>
        /// Create a fixed size uninitialized buffer.
        /// Use Enqueue / Dequeue / TryDequeue / Count / Capacity / Clear / Resize
        /// and the enumerator.
        /// </summary>
        /// <param name="fixedSize"></param>
        public CircularBuffer(uint fixedSize)
        {
            buffer = new T[fixedSize];
            front = -1;
            back = -1;

            enumeratorIndex = -1;
        }

        /// <summary>
        /// Remove all elements from the buffer.
        /// This doesn't actually remove the elements,
        /// only resets the queue front/back markers.
        /// Thus, the ref-count of objects doesn't go down
        /// on calling this function.
        /// </summary>
        public void Clear()
        {
            front = -1;
            back = -1;
        }

        /// <summary>
        /// Change the capacity of the buffer.
        /// If new capacity is more than the previous one, all
        /// old data is copied into the new buffer. If new
        /// capacity is less than previous one, only the latest
        /// newCapacity number of elements are preserved.
        /// </summary>
        /// <param name="newCapacity">New capacity</param>
        public void Resize(uint newCapacity)
        {
            if (newCapacity == Capacity)
            {
                return;
            }

            if (Count == 0)
            {
                buffer = new T[newCapacity];
                front = -1;
                back = -1;
            }
            else
            {
                T[] newBuffer = new T[newCapacity];
                // TODO : dequeue more efficiently
                int numWastedElements = (int)newCapacity - Capacity;
                for (uint i = 0; i < numWastedElements; ++i)
                {
                    TryDequeue(out T _);
                }

                Dequeue(newBuffer);
                buffer = newBuffer;
            }
        }

        /// <summary>
        /// Pushes a new element at the end of the queue.
        /// Will replace the front element if queue is full.
        /// TODO : ^ make this behavior configurable
        /// </summary>
        /// <param name="value">Value to enqueue</param>
        public void Enqueue(T value)
        {
            back = (back + 1) % buffer.Length;
            buffer[back] = value;

            if (back == front || front == -1)
            {
                front = (front + 1) % buffer.Length;
            }
        }

        /// <summary>
        /// Enqueue an entire array at the end of the queue.
        /// Uses Array.Copy() and is thus more efficient
        /// than calling Enqueue(T value) in a loop.
        /// </summary>
        /// <param name="values">Array of values to enqueue</param>
        public void Enqueue(T[] values)
        {
            // If we need to enqueue more values than the buffer size,
            // only enqueue the last buffer size number of elements.
            if (values.Length >= buffer.Length)
            {
                System.Array.Copy(values, values.Length - buffer.Length, buffer, 0, buffer.Length);
                front = 0;
                back = buffer.Length - 1;
            }
            else
            {
                if (front == -1)
                {
                    System.Array.Copy(values, 0, buffer, 0, values.Length);
                    front = 0;
                    back = values.Length - 1;
                }
                else
                {
                    int oldBack = back;
                    int remainingCapacityToEndOfBuffer = buffer.Length - back - 1;
                    if (values.Length <= remainingCapacityToEndOfBuffer)
                    {
                        System.Array.Copy(values, 0, buffer, back + 1, values.Length);
                        back += values.Length;
                        if (oldBack < front)
                        {
                            front = (back >= front) ? (back + 1) % buffer.Length : front;
                        }
                    }
                    // wraparound
                    else
                    {
                        System.Array.Copy(values, 0, buffer, back + 1, remainingCapacityToEndOfBuffer);

                        int remainingNumElementsToCopy = values.Length - remainingCapacityToEndOfBuffer;
                        System.Array.Copy(values, remainingCapacityToEndOfBuffer, buffer, 0, remainingNumElementsToCopy);

                        back = (remainingNumElementsToCopy - 1);

                        front = ((back >= front) || (oldBack < front)) ? (back + 1) % buffer.Length : front;
                    }
                }
            }
        }

        /// <summary>
        /// Attempt to dequeue a value from the front of the queue.
        /// </summary>
        /// <param name="value">Dequeued value</param>
        /// <returns>True if dequeue was successful, false otherwise</returns>
        public bool TryDequeue(out T value)
        {
            if (front == -1)
            {
                value = default;
                return false;
            }

            value = buffer[front];
            if (front == back)
            {
                front = -1;
                back = -1;
            }
            else
            {
                front = (front + 1) % buffer.Length;
            }

            return true;
        }

        /// <summary>
        /// Dequeue an entire array of elements.
        /// Uses Array.Copy() and is thus more efficient
        /// than calling TryDequeue() in a loop.
        /// </summary>
        /// <param name="data">Array to dequeue into</param>
        /// <returns>Number of elements that were dequeued into the provided array</returns>
        public int Dequeue(T[] data)
        {
            if (front == -1)
            {
                return 0;
            }

            if (front <= back)
            {
                int maxData = Math.Min(data.Length, this.Count);
                System.Array.Copy(buffer, front, data, 0, maxData);
                front += maxData;

                if (front > back)
                {
                    front = -1;
                    back = -1;
                }

                return maxData;
            }
            else
            {
                int numElementsToEndOfBuffer = buffer.Length - front;
                int numElementsDequeued = Math.Min(data.Length, numElementsToEndOfBuffer);
                System.Array.Copy(buffer, front, data, 0, numElementsDequeued);

                if (numElementsDequeued >= numElementsToEndOfBuffer)
                {
                    front = 0;
                }
                else
                {
                    front += numElementsDequeued;
                }

                if (numElementsDequeued < data.Length)
                {
                    int numElementsRemaining = back + 1;
                    int numPossibleElementsToDequeue = Math.Min(data.Length - numElementsDequeued, numElementsRemaining);
                    System.Array.Copy(buffer, 0, data, numElementsDequeued, numPossibleElementsToDequeue);

                    if (numPossibleElementsToDequeue >= numElementsRemaining)
                    {
                        front = -1;
                        back = -1;
                    }
                    else
                    {
                        front = numElementsDequeued;
                    }

                    return numElementsDequeued + numPossibleElementsToDequeue;
                }

                return numElementsDequeued;
            }
        }

        /// <summary>
        /// Move to the next element when using this class as an IEnumerator
        /// </summary>
        /// <returns>True if there are still more elements remaining in the queue, false if end of collection has been reached</returns>
        public bool MoveNext()
        {
            if (front == -1)
            {
                enumeratorIndex = -1;
                return false;
            }

            int oldEnumeratorIndex = enumeratorIndex;
            enumeratorIndex = (enumeratorIndex == -1) ? front : (enumeratorIndex + 1);
            if (front <= back)
            {
                return (enumeratorIndex >= front) && (enumeratorIndex <= back);
            }

            enumeratorIndex %= buffer.Length;
            // if front > back and enumerator has looped back to front
            if (enumeratorIndex == front && oldEnumeratorIndex == enumeratorIndex - 1)
            {
                return false;
            }
            return (enumeratorIndex >= front) || (enumeratorIndex <= back);
        }

        /// <summary>
        /// Reset the flags for the enumerator
        /// </summary>
        public void Reset()
        {
            enumeratorIndex = -1;
        }

        /// <summary>
        /// Get the enumerator implemented for this collection
        /// </summary>
        /// <returns>Enumerator</returns>
        public IEnumerator GetEnumerator()
        {
            Reset();
            return this;
        }

        /// <summary>
        /// Gets the curent pre-initialized object in use.
        /// </summary>
        /// <returns>Current object in the circular buffer</returns>
        public T Peek()
        {
            return buffer[bufferIndex];
        }

        /// <summary>
        /// Gets the next available pre-initialized object.
        /// </summary>
        /// <returns>Next object in the circular buffer</returns>
        public T Get()
        {
            T result = buffer[bufferIndex];

            bufferIndex++;
            if (bufferIndex >= buffer.Length)
            {
                bufferIndex = 0;
            }

            return result;
        }
    }
}
