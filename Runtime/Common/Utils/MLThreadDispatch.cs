// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap.Native
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;

    /// <summary>
    /// Handles dispatching calls from the Magic Leap native thread to the Unity thread
    /// </summary>
    public class MLThreadDispatch
    {
        /// <summary>
        /// A concurrent queue handles the dispatched callbacks in a thread-safe way.
        /// </summary>
        private static ConcurrentQueue<Dispatcher> actionQueue = new ConcurrentQueue<Dispatcher>();

        /// <summary>
        /// The concurrent queue for actions to execute on the main thread.
        /// </summary>
        private static ConcurrentQueue<System.Action> mainActionQueue = new ConcurrentQueue<System.Action>();

        /// <summary>
        /// The concurrent queue for actions to execute on the graphics thread.
        /// </summary>
        private static ConcurrentQueue<System.Action> graphicsActionQueue = new ConcurrentQueue<System.Action>();

        private static bool registeredForGraphicsCallbacks = false;

        /// <summary>
        /// The worker thread
        /// </summary>
        private static System.Threading.Thread thread = null;

        /// <summary>
        /// The concurrent queue of itemized work items that will execute on the worker thread.
        /// </summary>
        private static ConcurrentQueue<Func<bool>> itemizedWork = new ConcurrentQueue<Func<bool>>();

        /// <summary>
        /// A method that schedules a callback on the worker thread.
        /// </summary>
        /// <param name="function">Function to call. Return TRUE when processing is done, FALSE to be placed back in the queue to be called again at a later time.</param>
        public static void ScheduleWork(Func<bool> function)
        {
            itemizedWork.Enqueue(function);
        }

        /// <summary>
        /// A method that schedules a callback on the main thread.
        /// </summary>
        /// <param name="callback">A callback function to be called when the action is invoked </param>
        public static void ScheduleMain(System.Action callback)
        {
            if (MLDevice.Instance != null && MLDevice.MainThreadId != -1 && MLDevice.MainThreadId == System.Threading.Thread.CurrentThread.ManagedThreadId)
            {
                callback();
            }
            else
            {
                mainActionQueue.Enqueue(callback);
            }
        }

        public static void ScheduleGraphics(System.Action callback)
        {
            graphicsActionQueue.Enqueue(callback);
            if (!registeredForGraphicsCallbacks)
            {
                registeredForGraphicsCallbacks = true;
                MLGraphicsHooks.OnPreBeginRenderFrame += ExecuteOnPreBeginRenderFrameTasks;
            }
        }

        private static void ExecuteOnPreBeginRenderFrameTasks()
        {
            while (graphicsActionQueue.TryDequeue(out System.Action action))
            {
                action();
            }
        }

        /// <summary>
        /// A method that queues an action without a payload
        /// </summary>
        /// <param name="callback">A callback function to be called when the delegate is invoked </param>
        public static void Call(System.Delegate callback)
        {
            if (callback != null)
            {
                System.Action call = delegate
                {
                    DispatchNoPayload newDispatch = new DispatchNoPayload(Cast<Action>(callback));
                    actionQueue.Enqueue(newDispatch);
                };

                call();
            }
        }

        /// <summary>
        /// A method that queues an action without a payload
        /// </summary>
        /// <param name="callback">A callback function to be called when the action is invoked </param>
        public static void Call(System.Action callback)
        {
            if (callback != null)
            {
                System.Action call = delegate
                {
                    DispatchNoPayload newDispatch = new DispatchNoPayload(callback);
                    actionQueue.Enqueue(newDispatch);
                };
                call();
            }
        }

        /// <summary>
        /// A template method that queues an action with a single payload
        /// </summary>
        /// <typeparam name="A">Payload type</typeparam>
        /// <param name="a">Payload 1</param>
        /// <param name="callback">A callback function to be called when the delegate is invoked </param>
        public static void Call<A>(A a, System.Delegate callback)
        {
            if (callback != null)
            {
                System.Action<A> call = delegate (A arg1)
                {
                    DispatchPayload1<A> newDispatch = new DispatchPayload1<A>(arg1, Cast<Action<A>>(callback));
                    actionQueue.Enqueue(newDispatch);
                };

                call(a);
            }
        }

        /// <summary>
        /// A template method that queues an action with two payloads
        /// </summary>
        /// <typeparam name="A">First payload type</typeparam>
        /// <typeparam name="B">Second payload type</typeparam>
        /// <param name="a">Payload 1</param>
        /// <param name="b">Payload 2</param>
        /// <param name="callback">A callback function to be called when the delegate is invoked </param>
        public static void Call<A, B>(A a, B b, System.Delegate callback)
        {
            if (callback != null)
            {
                System.Action<A, B> call = delegate (A arg1, B arg2)
                {
                    DispatchPayload2<A, B> newDispatch = new DispatchPayload2<A, B>(arg1, arg2, Cast<Action<A, B>>(callback));
                    actionQueue.Enqueue(newDispatch);
                };

                call(a, b);
            }
        }

        /// <summary>
        /// A template method that queues an action with three payloads
        /// </summary>
        /// <typeparam name="A">First payload type</typeparam>
        /// <typeparam name="B">Second payload type</typeparam>
        /// <typeparam name="C">Third payload type</typeparam>
        /// <param name="a">Payload 1</param>
        /// <param name="b">Payload 2</param>
        /// <param name="c">Payload 3</param>
        /// <param name="callback">A callback function to be called when the delegate is invoked </param>
        public static void Call<A, B, C>(A a, B b, C c, System.Delegate callback)
        {
            if (callback != null)
            {
                System.Action<A, B, C> call = delegate (A arg1, B arg2, C arg3)
                {
                    DispatchPayload3<A, B, C> newDispatch = new DispatchPayload3<A, B, C>(arg1, arg2, arg3, Cast<Action<A, B, C>>(callback));
                    actionQueue.Enqueue(newDispatch);
                };

                call(a, b, c);
            }
        }

        /// <summary>
        /// A template method that queues an action with four payloads
        /// </summary>
        /// <typeparam name="A">First payload type</typeparam>
        /// <typeparam name="B">Second payload type</typeparam>
        /// <typeparam name="C">Third payload type</typeparam>
        /// <typeparam name="D">Forth payload type</typeparam>
        /// <param name="a">Payload 1</param>
        /// <param name="b">Payload 2</param>
        /// <param name="c">Payload 3</param>
        /// <param name="d">Payload 4</param>
        /// <param name="callback">A callback function to be called when the delegate is invoked </param>
        public static void Call<A, B, C, D>(A a, B b, C c, D d, System.Delegate callback)
        {
            if (callback != null)
            {
                System.Action<A, B, C, D> call = delegate (A arg1, B arg2, C arg3, D arg4)
                {
                    DispatchPayload4<A, B, C, D> newDispatch = new DispatchPayload4<A, B, C, D>(arg1, arg2, arg3, arg4, Cast<Action<A, B, C, D>>(callback));
                    actionQueue.Enqueue(newDispatch);
                };

                call(a, b, c, d);
            }
        }

        /// <summary>
        /// A template method that queues an action with five payloads
        /// </summary>
        /// <typeparam name="A">First payload type</typeparam>
        /// <typeparam name="B">Second payload type</typeparam>
        /// <typeparam name="C">Third payload type</typeparam>
        /// <typeparam name="D">Forth payload type</typeparam>
        /// <typeparam name="E">Fifth payload type</typeparam>
        /// <param name="a">Payload 1</param>
        /// <param name="b">Payload 2</param>
        /// <param name="c">Payload 3</param>
        /// <param name="d">Payload 4</param>
        /// <param name="e">Payload 5</param>
        /// <param name="callback">A callback function to be called when the delegate is invoked </param>
        public static void Call<A, B, C, D, E>(A a, B b, C c, D d, E e, System.Delegate callback)
        {
            if (callback != null)
            {
                System.Action<A, B, C, D, E> call = delegate (A arg1, B arg2, C arg3, D arg4, E arg5)
                {
                    DispatchPayload5<A, B, C, D, E> newDispatch = new DispatchPayload5<A, B, C, D, E>(arg1, arg2, arg3, arg4, arg5, Cast<Action<A, B, C, D, E>>(callback));
                    actionQueue.Enqueue(newDispatch);
                };

                call(a, b, c, d, e);
            }
        }

        /// <summary>
        /// A template method that queues an action with six payloads
        /// </summary>
        /// <typeparam name="A">First payload type</typeparam>
        /// <typeparam name="B">Second payload type</typeparam>
        /// <typeparam name="C">Third payload type</typeparam>
        /// <typeparam name="D">Forth payload type</typeparam>
        /// <typeparam name="E">Fifth payload type</typeparam>
        /// <typeparam name="F">Sixth payload type</typeparam>
        /// <param name="a">Payload 1</param>
        /// <param name="b">Payload 2</param>
        /// <param name="c">Payload 3</param>
        /// <param name="d">Payload 4</param>
        /// <param name="e">Payload 5</param>
        /// <param name="f">Payload 6</param>
        /// <param name="callback">A callback function to be called when the delegate is invoked </param>
        public static void Call<A, B, C, D, E, F>(A a, B b, C c, D d, E e, F f, System.Delegate callback)
        {
            if (callback != null)
            {
                System.Action<A, B, C, D, E, F> call = delegate (A arg1, B arg2, C arg3, D arg4, E arg5, F arg6)
                {
                    DispatchPayload6<A, B, C, D, E, F> newDispatch = new DispatchPayload6<A, B, C, D, E, F>(arg1, arg2, arg3, arg4, arg5, arg6, Cast<Action<A, B, C, D, E, F>>(callback));
                    actionQueue.Enqueue(newDispatch);
                };

                call(a, b, c, d, e, f);
            }
        }

        /// <summary>
        /// Dispatch all queued items
        /// </summary>
        public static void DispatchAll()
        {
            Dispatcher callbacks;
            System.Action action;

            while (actionQueue.TryDequeue(out callbacks))
            {
                callbacks.Dispatch();
            }

            while (mainActionQueue.TryDequeue(out action))
            {
                action();
            }

            if (thread == null && !itemizedWork.IsEmpty)
            {
                thread = new Thread(ExecuteBackgroundThread);
                thread.Start();
            }
        }

        /// <summary>
        /// Casts a delegate of unknown type to a delegate of a defined type.
        /// </summary>
        /// <typeparam name="T">Type of delegate to cast to</typeparam>
        /// <param name="source">Original delegate</param>
        /// <returns>Returns a newly constructed delegate of the specified type or null if a conversion doesn't exist</returns>
        private static T Cast<T>(Delegate source) where T : class
        {
            if (source == null)
            {
                return null;
            }

            Delegate[] delegates = source.GetInvocationList();

            if (delegates.Length > 1)
            {
                Delegate[] delegatesDest = new Delegate[delegates.Length];

                for (int nn = 0; nn < delegates.Length; nn++)
                {
                    delegatesDest[nn] = Delegate.CreateDelegate(typeof(T), delegates[nn].Target, delegates[nn].Method);
                }

                return Delegate.Combine(delegatesDest) as T;
            }

            return Delegate.CreateDelegate(typeof(T), delegates[0].Target, delegates[0].Method) as T;
        }

        /// <summary>
        /// Static method that executes the background worker thread.
        /// </summary>
        /// <param name="obj">Optional object</param>
        private static void ExecuteBackgroundThread(object obj)
        {
            Thread.CurrentThread.IsBackground = true;

            while (true)
            {
                Func<bool> function;

                if (itemizedWork.TryDequeue(out function))
                {
                    bool result = function();

                    if (!result)
                    {
                        // Not done yet. Put it at the end of the queue to try again later.
                        itemizedWork.Enqueue(function);
                    }
                }
                else
                {
                    // Yield a reasonable timeslice.
                    Thread.Sleep(5);
                }
            }
        }

        /// <summary>
        /// Defines a generic dispatching class.
        /// </summary>
        private abstract class Dispatcher
        {
            /// <summary>
            /// Abstract dispatch method to be called when removing callbacks from the queue
            /// </summary>
            public abstract void Dispatch();
        }

        /// <summary>
        /// Overloads the generic dispatcher to call back a method without a payload
        /// </summary>
        private class DispatchNoPayload : Dispatcher
        {
            /// <summary>
            /// Stores the method to be dispatched
            /// </summary>
            private System.Action action;

            /// <summary>
            /// Initializes a new instance of the <see cref="DispatchNoPayload"/> class with the supplied callback
            /// </summary>
            /// <param name="action">Method to call back</param>
            public DispatchNoPayload(System.Action action)
            {
                this.action = action;
            }

            /// <summary>
            /// Dispatches the previously stored callback
            /// </summary>
            public override void Dispatch()
            {
                this.action();
            }
        }

        /// <summary>
        /// Overloads the generic dispatcher to call back a method with a single payload
        /// </summary>
        /// <typeparam name="T">Payload type</typeparam>
        private class DispatchPayload1<T> : Dispatcher
        {
            /// <summary>
            /// Stores the method to be dispatched
            /// </summary>
            private System.Action<T> action;

            /// <summary>
            /// Stores a copy or reference of the payload to dispatch
            /// </summary>
            private T payload;

            /// <summary>
            /// Initializes a new instance of the <see cref="DispatchPayload1{T}"/> class. with the supplied callback and payload
            /// </summary>
            /// <param name="payload">Payload to dispatch</param>
            /// <param name="action">Method to call back</param>
            public DispatchPayload1(T payload, System.Action<T> action)
            {
                this.payload = payload;
                this.action = action;
            }

            /// <summary>
            /// Dispatches the previously stored callback with the supplied payload
            /// </summary>
            public override void Dispatch()
            {
                this.action(this.payload);
            }
        }

        /// <summary>
        /// Overloads the generic dispatcher to call back a method with two payloads
        /// </summary>
        /// <typeparam name="A">First payload type</typeparam>
        /// <typeparam name="B">Second payload type</typeparam>
        private class DispatchPayload2<A, B> : Dispatcher
        {
            /// <summary>
            /// Stores the method to be dispatched
            /// </summary>
            private System.Action<A, B> action;

            /// <summary>
            /// Stores a copy or reference of a payload to dispatch
            /// </summary>
            private A payload1;

            /// <summary>
            /// Stores a copy or reference of a payload to dispatch
            /// </summary>
            private B payload2;

            /// <summary>
            /// Initializes a new instance of the <see cref="DispatchPayload2{A,B}"/> class with the supplied callback and payloads
            /// </summary>
            /// <param name="payload1">First payload</param>
            /// <param name="payload2">Second payload</param>
            /// <param name="action">Method to dispatch</param>
            public DispatchPayload2(A payload1, B payload2, System.Action<A, B> action)
            {
                this.payload1 = payload1;
                this.payload2 = payload2;
                this.action = action;
            }

            /// <summary>
            /// Dispatches the previously stored callback with the supplied payloads
            /// </summary>
            public override void Dispatch()
            {
                this.action(this.payload1, this.payload2);
            }
        }

        /// <summary>
        /// Overloads the generic dispatcher to call back a method with three payloads
        /// </summary>
        /// <typeparam name="A">First payload type</typeparam>
        /// <typeparam name="B">Second payload type</typeparam>
        /// <typeparam name="C">Third payload type</typeparam>
        private class DispatchPayload3<A, B, C> : Dispatcher
        {
            /// <summary>
            /// Stores the method to be dispatched
            /// </summary>
            private System.Action<A, B, C> action;

            /// <summary>
            /// Stores a copy or reference of a payload to dispatch
            /// </summary>
            private A payload1;

            /// <summary>
            /// Stores a copy or reference of a payload to dispatch
            /// </summary>
            private B payload2;

            /// <summary>
            /// Stores a copy or reference of a payload to dispatch
            /// </summary>
            private C payload3;

            /// <summary>
            /// Initializes a new instance of the <see cref="DispatchPayload3{A,B,C}"/> class with the supplied callback and payloads
            /// </summary>
            /// <param name="payload1">First payload</param>
            /// <param name="payload2">Second payload</param>
            /// <param name="payload3">Third payload</param>
            /// <param name="action">Method to dispatch</param>
            public DispatchPayload3(A payload1, B payload2, C payload3, System.Action<A, B, C> action)
            {
                this.payload1 = payload1;
                this.payload2 = payload2;
                this.payload3 = payload3;
                this.action = action;
            }

            /// <summary>
            /// Dispatches the previously stored callback with the supplied payloads
            /// </summary>
            public override void Dispatch()
            {
                this.action(this.payload1, this.payload2, this.payload3);
            }
        }

        /// <summary>
        /// Overloads the generic dispatcher to call back a method with four payloads
        /// </summary>
        /// <typeparam name="A">First payload type</typeparam>
        /// <typeparam name="B">Second payload type</typeparam>
        /// <typeparam name="C">Third payload type</typeparam>
        /// <typeparam name="D">Forth payload type</typeparam>
        private class DispatchPayload4<A, B, C, D> : Dispatcher
        {
            /// <summary>
            /// Stores the method to be dispatched
            /// </summary>
            private System.Action<A, B, C, D> action;

            /// <summary>
            /// Stores a copy or reference of a payload to dispatch
            /// </summary>
            private A payload1;

            /// <summary>
            /// Stores a copy or reference of a payload to dispatch
            /// </summary>
            private B payload2;

            /// <summary>
            /// Stores a copy or reference of a payload to dispatch
            /// </summary>
            private C payload3;

            /// <summary>
            /// Stores a copy or reference of a payload to dispatch
            /// </summary>
            private D payload4;

            /// <summary>
            /// Initializes a new instance of the <see cref="DispatchPayload4{A,B,C,D}"/> class with the supplied callback and payloads
            /// </summary>
            /// <param name="payload1">First payload</param>
            /// <param name="payload2">Second payload</param>
            /// <param name="payload3">Third payload</param>
            /// <param name="payload4">Forth payload</param>
            /// <param name="action">Method to dispatch</param>
            public DispatchPayload4(A payload1, B payload2, C payload3, D payload4, System.Action<A, B, C, D> action)
            {
                this.payload1 = payload1;
                this.payload2 = payload2;
                this.payload3 = payload3;
                this.payload4 = payload4;
                this.action = action;
            }

            /// <summary>
            /// Dispatches the previously stored callback with the supplied payloads
            /// </summary>
            public override void Dispatch()
            {
                this.action(this.payload1, this.payload2, this.payload3, this.payload4);
            }
        }

        /// <summary>
        /// Overloads the generic dispatcher to call back a method with four payloads
        /// </summary>
        /// <typeparam name="A">First payload type</typeparam>
        /// <typeparam name="B">Second payload type</typeparam>
        /// <typeparam name="C">Third payload type</typeparam>
        /// <typeparam name="D">Forth payload type</typeparam>
        /// <typeparam name="E">Fifth payload type</typeparam>
        private class DispatchPayload5<A, B, C, D, E> : Dispatcher
        {
            /// <summary>
            /// Stores the method to be dispatched
            /// </summary>
            private System.Action<A, B, C, D, E> action;

            /// <summary>
            /// Stores a copy or reference of a payload to dispatch
            /// </summary>
            private A payload1;

            /// <summary>
            /// Stores a copy or reference of a payload to dispatch
            /// </summary>
            private B payload2;

            /// <summary>
            /// Stores a copy or reference of a payload to dispatch
            /// </summary>
            private C payload3;

            /// <summary>
            /// Stores a copy or reference of a payload to dispatch
            /// </summary>
            private D payload4;

            /// <summary>
            /// Stores a copy or reference of a payload to dispatch
            /// </summary>
            private E payload5;

            /// <summary>
            /// Initializes a new instance of the <see cref="DispatchPayload5{A,B,C,D,F}"/> class with the supplied callback and payloads
            /// </summary>
            /// <param name="payload1">First payload</param>
            /// <param name="payload2">Second payload</param>
            /// <param name="payload3">Third payload</param>
            /// <param name="payload4">Forth payload</param>
            /// <param name="payload5">Fifth payload</param>
            /// <param name="action">Method to dispatch</param>
            public DispatchPayload5(A payload1, B payload2, C payload3, D payload4, E payload5, System.Action<A, B, C, D, E> action)
            {
                this.payload1 = payload1;
                this.payload2 = payload2;
                this.payload3 = payload3;
                this.payload4 = payload4;
                this.payload5 = payload5;
                this.action = action;
            }

            /// <summary>
            /// Dispatches the previously stored callback with the supplied payloads
            /// </summary>
            public override void Dispatch()
            {
                this.action(this.payload1, this.payload2, this.payload3, this.payload4, this.payload5);
            }
        }

        /// <summary>
        /// Overloads the generic dispatcher to call back a method with four payloads
        /// </summary>
        /// <typeparam name="A">First payload type</typeparam>
        /// <typeparam name="B">Second payload type</typeparam>
        /// <typeparam name="C">Third payload type</typeparam>
        /// <typeparam name="D">Forth payload type</typeparam>
        /// <typeparam name="E">Fifth payload type</typeparam>
        /// <typeparam name="E">Sixth payload type</typeparam>
        private class DispatchPayload6<A, B, C, D, E, F> : Dispatcher
        {
            /// <summary>
            /// Stores the method to be dispatched
            /// </summary>
            private System.Action<A, B, C, D, E, F> action;

            /// <summary>
            /// Stores a copy or reference of a payload to dispatch
            /// </summary>
            private A payload1;

            /// <summary>
            /// Stores a copy or reference of a payload to dispatch
            /// </summary>
            private B payload2;

            /// <summary>
            /// Stores a copy or reference of a payload to dispatch
            /// </summary>
            private C payload3;

            /// <summary>
            /// Stores a copy or reference of a payload to dispatch
            /// </summary>
            private D payload4;

            /// <summary>
            /// Stores a copy or reference of a payload to dispatch
            /// </summary>
            private E payload5;

            /// <summary>
            /// Stores a copy or reference of a payload to dispatch
            /// </summary>
            private F payload6;

            /// <summary>
            /// Initializes a new instance of the <see cref="DispatchPayload6{A,B,C,D,E,F}"/> class with the supplied callback and payloads
            /// </summary>
            /// <param name="payload1">First payload</param>
            /// <param name="payload2">Second payload</param>
            /// <param name="payload3">Third payload</param>
            /// <param name="payload4">Forth payload</param>
            /// <param name="payload5">Fifth payload</param>
            /// <param name="payload6">Sixth payload</param>
            /// <param name="action">Method to dispatch</param>
            public DispatchPayload6(A payload1, B payload2, C payload3, D payload4, E payload5, F payload6, System.Action<A, B, C, D, E, F> action)
            {
                this.payload1 = payload1;
                this.payload2 = payload2;
                this.payload3 = payload3;
                this.payload4 = payload4;
                this.payload5 = payload5;
                this.payload6 = payload6;
                this.action = action;
            }

            /// <summary>
            /// Dispatches the previously stored callback with the supplied payloads
            /// </summary>
            public override void Dispatch()
            {
                this.action(this.payload1, this.payload2, this.payload3, this.payload4, this.payload5, this.payload6);
            }
        }
    }
}
