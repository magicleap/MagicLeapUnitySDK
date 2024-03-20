using NUnit.Framework;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.XR.MagicLeap;

namespace UnitySDKPlayTests
{
    public class NativeRingBufferTests
    {
        [Test]
        public void DefaultNativeRingBufferIsInvalid()
        {
            var buffer = new NativeRingBuffer();
            Assert.That(buffer.IsCreated, Is.False);
        }

        [Test]
        public void NativeRingBufferWorksWithDifferentAllocators()
        {
            var tmpBuffer = new NativeRingBuffer(1024, Allocator.Temp);
            Assert.That(tmpBuffer.IsCreated, Is.True);
            tmpBuffer.Dispose();
            Assert.That(tmpBuffer.IsCreated, Is.False);

            var tmpJobBuffer = new NativeRingBuffer(1024, Allocator.TempJob);
            Assert.That(tmpJobBuffer.IsCreated, Is.True);
            tmpJobBuffer.Dispose();
            Assert.That(tmpJobBuffer.IsCreated, Is.False);

            var persistentBuffer = new NativeRingBuffer(1024, Allocator.Persistent);
            Assert.That(persistentBuffer.IsCreated, Is.True);
            persistentBuffer.Dispose();
            Assert.That(persistentBuffer.IsCreated, Is.False);
        }

        [TestCase(256)]
        [TestCase(512)]
        [TestCase(1024)]
        public void NativeRingBufferSizeMustBePowerOfTwo(long size)
        {
            var buffer = new NativeRingBuffer(size, Allocator.Temp);
            Assert.That(buffer.IsCreated, Is.True);
            buffer.Dispose();
            Assert.That(buffer.IsCreated, Is.False);
        }

        [TestCase(255)]
        [TestCase(511)]
        [TestCase(1023)]
        public void NativeRingBufferSizeMustBePowerOfTwoInvalid(long size)
        {
            Assert.That(() => new NativeRingBuffer(size, Allocator.Temp), Throws.ArgumentException);
        }

        [TestCase(5)]
        [TestCase(10)]
        public void NativeRingBufferWorksOnSingleThreadWithBlockingOps(int value)
        {
            using var buffer = new NativeRingBuffer(64, Allocator.Temp);
            Assert.That(buffer.IsCreated, Is.True);
            using var reader = buffer.AsBlockingReader();
            using var writer = buffer.AsBlockingWriter();

            writer.Write(value);
            writer.FinishWrite();
            Assert.That(reader.Read<int>(), Is.EqualTo(value));
            reader.FinishRead();
        }

        [TestCase(5)]
        [TestCase(10)]
        public void NativeRingBufferWorksOnSingleThreadWithNonblockingOps(int value)
        {
            using var buffer = new NativeRingBuffer(64, Allocator.Temp);
            Assert.That(buffer.IsCreated, Is.True);
            using var reader = buffer.AsNonblockingReader();
            using var writer = buffer.AsNonblockingWriter();

            Assert.That(writer.TryWrite(value), Is.True);
            writer.FinishWrite();
            Assert.That(reader.TryRead<int>(out var value2), Is.True);
            Assert.That(value2, Is.EqualTo(value));
            reader.FinishRead();
        }

        [Test]
        public void NativeRingBufferNonblockingReaderFailsUntilAWriteIsCompleted()
        {
            using var buffer = new NativeRingBuffer(64, Allocator.Temp);
            Assert.That(buffer.IsCreated, Is.True);
            using var reader = buffer.AsNonblockingReader();
            using var writer = buffer.AsBlockingWriter();

            int result = 0;
            // this read should fail, because there is nothing to read.
            Assert.That(reader.TryRead(out result), Is.False);

            writer.Write(5);
            writer.FinishWrite();

            // this read should succeed.
            Assert.That(reader.TryRead(out result), Is.True);
            Assert.That(result, Is.EqualTo(5));
            reader.FinishRead();
        }

        [Test]
        public void NativeRingBufferNonblockingWriterFailsWhenBufferIsFull()
        {
            const long kSize = 64;
            using var buffer = new NativeRingBuffer(kSize, Allocator.Temp);
            Assert.That(buffer.IsCreated, Is.True);
            using var reader = buffer.AsNonblockingReader();
            using var writer = buffer.AsNonblockingWriter();

            // fill the buffer.
            int i;
            for (i = 0; writer.TryWrite(i); ++i) {}
            Assert.That(i, Is.EqualTo(kSize / sizeof(int)));
            writer.FinishWrite();

            // now, drain the buffer.
            int j, result;
            for (j = 0; reader.TryRead(out result); ++j)
                Assert.That(result, Is.EqualTo(j));
            reader.FinishRead();

            // now, try to write to the buffer again.
            Assert.That(writer.TryWrite(5), Is.True);
            writer.FinishWrite();

            Assert.That(reader.TryRead(out result), Is.True);
            Assert.That(result, Is.EqualTo(5));
            reader.FinishRead();
        }

        private struct FillBufferJob : IJob
        {
            public NativeRingBuffer.NonblockingWriter writer;

            public void Execute()
            {
                int i = 0;
                while (writer.TryWrite(i++)) {}
                writer.FinishWrite();
            }
        }

        [Test]
        public void NativeBufferFillFromJobAndReadOnMainThread()
        {
            using var buffer = new NativeRingBuffer(64, Allocator.TempJob);
            Assert.That(buffer.IsCreated, Is.True);
            using var reader = buffer.AsNonblockingReader();
            using var writer = buffer.AsNonblockingWriter();

            // fill the buffer using a job.
            var job = new FillBufferJob { writer = writer };
            job.Schedule().Complete();

            // now, drain the buffer.
            int j, result;
            for (j = 0; reader.TryRead(out result); ++j)
                Assert.That(result, Is.EqualTo(j));
            reader.FinishRead();
        }
    }
}
