
using UnityEngine;

namespace MagicLeap.Android
{
    using System;
    using System.Collections.Generic;
    using UnityEngine.LowLevel;
    using UnityEngine.PlayerLoop;

    internal static class AndroidCameraPlayerLoopUtility
    {
        class UpdateSubscription<T> : IDisposable where T : class
        {
            private List<T> _List;
            private T _Obj;

            public UpdateSubscription(List<T> list, T obj)
                => (_List = list).Add(_Obj = obj);

            ~UpdateSubscription()
                => Dispose(false);

            public void Dispose()
                => Dispose(true);

            private void Dispose(bool disposing)
            {
                if (disposing && (_List == null || _Obj == null))
                    throw new ObjectDisposedException("object is already disposed");

                _List?.Remove(_Obj);
                _List = null;
                _Obj = null;
            }
        }

        // NB: a return value of `null` indicates the system is not installed in the playerloop,
        // while an empty array indicates it's a toplevel system in the playerloop.
        internal static Type[] GetInstallPathForSystem(in PlayerLoopSystem topLevelPlayerLoop, Type targetSystem)
        {
            var path = new List<Type>();
            return SearchPlayerLoopRecursive(topLevelPlayerLoop, targetSystem, path)
                ? path.ToArray()
                : null;
        }

        private static bool SearchPlayerLoopRecursive(in PlayerLoopSystem parent, Type targetSystem,
            List<Type> systemPath)
        {
            var subsystems = parent.subSystemList ?? Array.Empty<PlayerLoopSystem>();

            foreach (var currentSystem in subsystems)
            {
                if (currentSystem.type == targetSystem)
                {
                    systemPath.Insert(0, targetSystem);
                    return true;
                }

                if (!SearchPlayerLoopRecursive(currentSystem, targetSystem, systemPath))
                    continue;
                systemPath.Insert(0, currentSystem.type);
                return true;
            }

            return false;
        }

        internal static IDisposable LazyRegisterPlayerLoopUpdateInternal<T>(ref List<T> list, T obj, Type updateType,
            PlayerLoopSystem.UpdateFunction updateFunction, Type parentSystem, int index = -1) where T : class
        {
            if (list == null)
            {
                list = new List<T>();
                var updateSystem = new PlayerLoopSystem
                {
                    subSystemList = Array.Empty<PlayerLoopSystem>(),
                    type = updateType,
                    updateDelegate = updateFunction,
                };
                var playerLoop = PlayerLoop.GetCurrentPlayerLoop();

                if (!InstallIntoPlayerLoopAtIndex(ref playerLoop, updateSystem, parentSystem, index))
                    throw new Exception("unable to install update system into player loop!");
                PlayerLoop.SetPlayerLoop(playerLoop);
            }

            return new UpdateSubscription<T>(list, obj);
        }

        internal static bool InstallIntoPlayerLoopAtIndex(ref PlayerLoopSystem topLevelPlayerLoop,
            PlayerLoopSystem systemToInstall, Type parentSystem, int index = -1)
        {
            if (parentSystem == null)
                throw new ArgumentNullException(nameof(parentSystem));

            var installPath = GetInstallPathForSystem(topLevelPlayerLoop, parentSystem);
            if (installPath == null)
                throw new Exception(
                    $"'{parentSystem.Name}' doesn't appear to be installed in the current player loop");

            ref var current = ref topLevelPlayerLoop;
            foreach (var path in installPath)
            {
                var idx = Array.FindIndex(current.subSystemList, s => s.type == path);
                if (idx == -1)
                    return false;
                current = ref current.subSystemList[idx];
            }

            InstallSystemAtIndex(ref current, systemToInstall, index);
            return true;
        }

        private static void InstallSystemAtIndex(ref PlayerLoopSystem parentSystem, PlayerLoopSystem targetSystem,
            int installIndex = -1)
        {
            var subsystems = parentSystem.subSystemList ?? Array.Empty<PlayerLoopSystem>();
            ArrayUtility.ResizeAndInsert(ref subsystems, targetSystem, installIndex);
            parentSystem.subSystemList = subsystems;
        }
    }

    internal static class ArrayUtility
    {
        public static void ResizeAndInsert<T>(ref T[] array, T element, int index)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            var oldLength = array.Length;
            var newLength = oldLength + 1;
            var resolvedIndex = index < 0 ? newLength + index : index;
            if (resolvedIndex < 0 || resolvedIndex >= newLength)
                throw new ArgumentOutOfRangeException(nameof(index), $"argument '{nameof(index)}' resolved to {resolvedIndex}, which must be within 0 and {oldLength}, inclusive");
            Array.Resize(ref array, newLength);
            var inserted = false;
            var i = oldLength;
            while (!inserted)
            {
                if (i == resolvedIndex)
                {
                    array[i] = element;
                    inserted = true;
                }
                else
                {
                    // move items forward one spot to make room for new element.
                    array[i] = array[i - 1];
                }

                --i;
            }
        }
    }
}
