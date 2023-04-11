using System;
using System.Linq;
using UnityEngine;

namespace UnitySDKEditorTests
{
    public class MagicLeapNativeBindingsStructSizeTests
    {
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
            return $"The sizes between c-api and sdk for struct {structName}, do not match, {capiSize} =/= {sdkSize}.";
        }
    }
}