// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System;

    public abstract class MLLazySingleton<T> where T : MLLazySingleton<T>, new()
    {
        /// <summary>7
        /// DO NOT USE THIS! This class cannot be instantiated manually. 
        /// </summary>
        protected MLLazySingleton()
        {
            if (instance.IsValueCreated)
                throw new InvalidInstanceException($"Manually creating an instance of {typeof(T).Name} is not supported. You should use {typeof(T).Name}.Instance instead.");
        }

        protected static T Instance
        {
            get
            {
                if (!instance.IsValueCreated)
                    instance.Value.Initialize();

                return instance.Value;
            }
        }

        private static Lazy<T> instance = new Lazy<T>(true);

        protected virtual void Initialize() { }
    }

}
