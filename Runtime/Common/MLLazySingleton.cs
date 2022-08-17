// %BANNER_BEGIN% 
// --------------------------------------------------------------------- 
// %COPYRIGHT_BEGIN%
// <copyright file="MLLazySingleton.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
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
#if UNITY_MAGICLEAP || UNITY_ANDROID
            if (instance.IsValueCreated)
                throw new InvalidInstanceException($"Manually creating an instance of {typeof(T).Name} is not supported. You should use {typeof(T).Name}.Instance instead.");
#endif
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
