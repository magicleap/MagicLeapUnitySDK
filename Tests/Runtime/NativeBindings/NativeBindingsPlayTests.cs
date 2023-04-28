using System;
using System.Reflection;
using System.Runtime.InteropServices;
using NUnit.Framework;
using UnityEngine;

namespace UnitySDKPlayTests
{
    public class NativeBindingsPlayTests
    {
        protected Type nativeBindings;

        protected void AssertThatMethodExists(string methodName)
        {
            MethodInfo method = nativeBindings.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
            
            // Asserts the C# API defines the method as declared in the header file
            Assert.IsNotNull(method, $"NativeBinding extern method \"{methodName}\" is not defined.");

            try
            {
                method.Invoke(null, new object[method.GetParameters().Length]);
            }
            catch (TargetInvocationException ex)
            {
                // Asserts the method was included in the lib
                if (ex.InnerException is EntryPointNotFoundException)
                {
                    DllImportAttribute dllImportAttribute = (DllImportAttribute) method.GetCustomAttribute(typeof(DllImportAttribute));
                    Assert.Fail($"Method {methodName} not found in library {dllImportAttribute.Value}.");
                }
                
                if (ex.InnerException is MarshalDirectiveException)
                {
                    DllImportAttribute dllImportAttribute = (DllImportAttribute) method.GetCustomAttribute(typeof(DllImportAttribute));
                    Assert.Fail($"Method {methodName} is {ex.InnerException.Message} in library {dllImportAttribute.Value}.");
                }

                // some other exception
                Assert.Fail(ex.Message + " " + ex.InnerException?.ToString());
            }
        }
    }
}