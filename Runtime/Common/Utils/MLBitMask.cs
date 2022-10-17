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
    using UnityEngine;

    /// <summary>
    /// Custom attribute to make it easy to turn enum fields into bit masks in
    /// the inspector. The enum type must be defined in order for the inspector
    /// to be able to know what the bits should be set to.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class MLBitMask : PropertyAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MLBitMask"/> class.
        /// enum Type. This constructor call is automatic when
        /// decorating a field with this Attribute.
        /// </summary>
        /// <param name="propertyType">The Type value of the enum</param>
        public MLBitMask(Type propertyType)
        {
            this.PropertyType = propertyType;
        }

        /// <summary>
        /// Gets or sets the type of the Enum that is being turned into a bit mask.
        /// </summary>
        public Type PropertyType { get; private set; }
    }
}
