
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

        private static Type[] _InstallPath = {
            typeof(Initialization),
            typeof(Initialization.XREarlyUpdate)
        };



        internal static IDisposable LazyRegisterPlayerLoopUpdateInternal<T>(ref List<T> list, T obj, Type updateType,
            PlayerLoopSystem.UpdateFunction updateFunction, params Type[] installPath) where T : class
        {
            if (installPath == null || installPath.Length == 0)
                installPath = _InstallPath;
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
                if (!InstallIntoPlayerLoop(ref playerLoop, updateSystem, installPath))
                    throw new Exception("unable to install update system into player loop!");
                PlayerLoop.SetPlayerLoop(playerLoop);
            }

            return new UpdateSubscription<T>(list, obj);
        }

        private static bool InstallIntoPlayerLoop(ref PlayerLoopSystem topLevelPlayerLoop, PlayerLoopSystem systemToInstall, params Type[] installPath)
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
    }
}
