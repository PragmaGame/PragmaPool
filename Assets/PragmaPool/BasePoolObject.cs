using System;

namespace Pragma.Pool
{
    public abstract class BasePoolObject : IPoolObject
    {
        public Action<IPoolObject> ReleaseRequestAction { get; set; }

        public abstract void OnSpawn();

        public abstract void OnRelease();
    }
}