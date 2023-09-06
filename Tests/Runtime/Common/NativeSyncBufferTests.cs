using NUnit.Framework;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.XR.MagicLeap;

namespace UnitySDKPlayTests
{
    public class NativeSyncBufferTests
    {
        [Test]
        public void DefaultNativeSyncBufferIsInvalid()
        {
            var buffer = new NativeSyncBuffer<float>();
            Assert.That(buffer.IsValid, Is.False);
        }

        [Test]
        public void CreateBufferWithDifferentAllocatorsWorks()
        {
            NativeSyncBuffer<float> buffer;

            buffer = new NativeSyncBuffer<float>(Allocator.Temp);
            Assert.That(buffer.IsValid, Is.True);
            buffer.Dispose();
            Assert.That(buffer.IsValid, Is.False);

            buffer = new NativeSyncBuffer<float>(Allocator.TempJob);
            Assert.That(buffer.IsValid, Is.True);
            buffer.Dispose();
            Assert.That(buffer.IsValid, Is.False);

            buffer = new NativeSyncBuffer<float>(Allocator.Persistent);
            Assert.That(buffer.IsValid, Is.True);
            buffer.Dispose();
            Assert.That(buffer.IsValid, Is.False);
        }

        [Test]
        public void NewBufferHasDefaultValuesUnlessSpecified()
        {
            NativeSyncBuffer<int> buffer;

            buffer = new NativeSyncBuffer<int>(Allocator.Temp);
            AssertBufferHasValues(buffer, 0, 0);

            buffer.Dispose();

            buffer = new NativeSyncBuffer<int>(Allocator.Temp, 5);
            AssertBufferHasValues(buffer, 5, 5);

            buffer.Dispose();
        }

        [Test]
        public void UpdatingTheBufferChangesTheInputSideButNotTheOutputSide()
        {
            NativeSyncBuffer<int> buffer;

            buffer = new NativeSyncBuffer<int>(Allocator.Temp);
            buffer.UpdateInput(5);
            AssertBufferHasValues(buffer, 5, 0);

            buffer.Dispose();
        }

        [Test]
        public void SynchronizingTheBufferActuallyUpdatesTheOutputSide()
        {
            NativeSyncBuffer<int> buffer;

            buffer = new NativeSyncBuffer<int>(Allocator.Temp);
            buffer.UpdateInput(5);
            buffer.Sync();
            AssertBufferHasValues(buffer, 5, 5);

            buffer.Dispose();
        }

        private struct SyncBufferJob : IJob
        {
            public NativeSyncBuffer<int> Buffer;

            public void Execute() => Buffer.Sync();
        }

        private struct UpdateBufferJob : IJob
        {
            public NativeSyncBuffer<int> Buffer;
            public int Value;

            public void Execute() => Buffer.UpdateInput(Value);
        }

        [Test]
        public void CanUpdateBufferInputFromJob()
        {
            NativeSyncBuffer<int> buffer;

            buffer = new NativeSyncBuffer<int>(Allocator.TempJob);
            new UpdateBufferJob
            {
                Buffer = buffer,
                Value = 5,
            }.Schedule().Complete();
            AssertBufferHasValues(buffer, 5, 0);

            buffer.Dispose();
        }

        [Test]
        public void BufferAsyncUpdateWorks()
        {
            NativeSyncBuffer<int> buffer;

            buffer = new NativeSyncBuffer<int>(Allocator.TempJob);
            buffer.UpdateInputAsync(5, default).Complete();
            AssertBufferHasValues(buffer, 5, 0);
            buffer.Dispose();
        }

        [Test]
        public void CanSyncBufferOutputFromJob()
        {
            NativeSyncBuffer<int> buffer;

            buffer = new NativeSyncBuffer<int>(Allocator.TempJob);
            buffer.UpdateInput(5);
            AssertBufferHasValues(buffer,5,0);
            new SyncBufferJob { Buffer = buffer}.Schedule().Complete();
            AssertBufferHasValues(buffer,5,5);

            buffer.Dispose();
        }

        [Test]
        public void BufferAsnycSyncWorks()
        {
            NativeSyncBuffer<int> buffer;

            buffer = new NativeSyncBuffer<int>(Allocator.TempJob);
            buffer.UpdateInput(5);
            buffer.Sync(default).Complete();
            AssertBufferHasValues(buffer, 5, 5);

            buffer.Dispose();
        }

        [Test]
        public void BothAsyncUpdateAndSyncWork()
        {
            NativeSyncBuffer<int> buffer;

            buffer = new NativeSyncBuffer<int>(Allocator.TempJob);

            var handle = buffer.UpdateInputAsync(5, default);
            buffer.Sync(handle).Complete();

            AssertBufferHasValues(buffer, 5,5);

            buffer.Dispose();
        }

        [Test]
        public void TryingToUpdateAndSyncAtTheSameTimeFails()
        {
            NativeSyncBuffer<int> buffer;

            buffer = new NativeSyncBuffer<int>(Allocator.TempJob);
            var handle1 = buffer.UpdateInputAsync(5, default);
            Assert.That(() => buffer.Sync(), Throws.InvalidOperationException);
            handle1.Complete();
            AssertBufferHasValues(buffer, 5, 0);

            buffer.Dispose();

        }

        private void AssertBufferHasValues<T>(NativeSyncBuffer<T> buffer, in T input, in T output) where T : unmanaged
        {
            unsafe
            {
                Assert.That(*buffer.GetInputPointer(), Is.EqualTo(input));
                Assert.That(buffer.Output, Is.EqualTo(output));
            }
        }
    }
}
