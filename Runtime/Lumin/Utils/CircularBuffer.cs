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

namespace UnityEngine.XR.MagicLeap
{
    public class CircularBuffer<T>
    {
        private T[] buffer = null;
        private int bufferIndex = 0;

        public static CircularBuffer<T> Create(params T[] objects)
        {
            CircularBuffer<T> circularBuffer = new CircularBuffer<T>();
            circularBuffer.buffer = objects;
            return circularBuffer;
        }

        public T Get()
        {
            if (bufferIndex >= buffer.Length)
            {
                bufferIndex = 0;
            }

            return buffer[bufferIndex++];
        }
    }
}
