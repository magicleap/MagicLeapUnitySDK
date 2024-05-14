using System;
using System.Linq;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace MagicLeap
{
    internal class PlayerLoopUtil : MonoBehaviour
    {
        internal static Type[] InstallPath = {
            typeof(Initialization),
            typeof(Initialization.XREarlyUpdate)
        };

        internal static bool InstallIntoPlayerLoop(ref PlayerLoopSystem topLevelPlayerLoop, PlayerLoopSystem systemToInstall, params Type[] installPath)
        {
            installPath ??= Array.Empty<Type>();

            ref var current = ref topLevelPlayerLoop;
            foreach (var path in installPath)
            {
                var idx = Array.FindIndex(current.subSystemList, s => s.type == path);
                if (idx == -1)
                    return false;
                current = ref current.subSystemList[idx];
            }

            InstallSystem(ref current, systemToInstall);
            return true;
        }

        private static void InstallSystem(ref PlayerLoopSystem parentSystem, PlayerLoopSystem targetSystem)
        {
            var subsystems = parentSystem.subSystemList ?? Array.Empty<PlayerLoopSystem>();
            var length = subsystems.Length;
            Array.Resize(ref subsystems, length + 1);
            subsystems[length] = targetSystem;
            parentSystem.subSystemList = subsystems;
        }
        
        internal static void RemoveFromPlayerLoop(ref PlayerLoopSystem playerLoop, PlayerLoopSystem systemToRemove, params Type[] installPath)
        {
            installPath ??= Array.Empty<Type>();
            ref var current = ref playerLoop;
            foreach (var path in installPath)
            {
                var idx = Array.FindIndex(current.subSystemList, s => s.type == path);
                if (idx == -1)
                    return;
                current = ref current.subSystemList[idx];
            }

            var subsystems = current.subSystemList ?? Array.Empty<PlayerLoopSystem>();
            var subsystemList = subsystems.ToList();
            subsystemList.RemoveAll((targetSystem) => targetSystem.type == systemToRemove.type);
            current.subSystemList = subsystemList.ToArray();
        }
    }
}
