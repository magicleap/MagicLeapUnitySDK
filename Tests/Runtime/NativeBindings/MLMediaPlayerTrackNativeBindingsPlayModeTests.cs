using System.Reflection;
using NUnit.Framework;

namespace UnitySDKPlayTests
{
    public partial class MLMedia
    {
        public partial class Player
        {
            [Test]
            public void NativeBinding_FreeUnmanagedMemory_Exists()
            {
                AssertThatMethodExists("FreeUnmanagedMemory");
            }
        }
  
    }
}