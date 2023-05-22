using NUnit.Framework;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace UnitySDKEditorTests
{
    public class NativeBindingsTests
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
                    DllImportAttribute dllImportAttribute = (DllImportAttribute)method.GetCustomAttribute(typeof(DllImportAttribute));
                    Assert.Fail($"Method {methodName} not found in library {dllImportAttribute.Value}.");
                }

                if (ex.InnerException is MarshalDirectiveException)
                {
                    DllImportAttribute dllImportAttribute = (DllImportAttribute)method.GetCustomAttribute(typeof(DllImportAttribute));
                    Assert.Fail($"Method {methodName} is {ex.InnerException.Message} in library {dllImportAttribute.Value}.");
                }

                // some other exception
                Assert.Fail(ex.Message + " " + ex.InnerException?.ToString());
            }
        }

        protected Type FindTypeByName(string name)
        {
            var type = AppDomain.CurrentDomain.GetAssemblies().Reverse().Select(assembly => assembly.GetType(name))
                .FirstOrDefault(t => t != null);

            if (type != null)
                return type;

            return AppDomain.CurrentDomain.GetAssemblies().Reverse().SelectMany(assembly => assembly.GetTypes())
                .FirstOrDefault(t => t.Name.Equals(name));
        }

        protected string Log(string structName, ulong capiSize, int sdkSize)
        {
            return $"Size mismatch for struct {structName}, CAPI: {capiSize}, C#: {sdkSize}.";
        }
    }
}
