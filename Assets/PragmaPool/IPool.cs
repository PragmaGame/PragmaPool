using System;
using System.Collections.Generic;

namespace Pragma.Pool
{
    public interface IPool
    {
        public void Release(IPoolObject instance);
        public void ReleaseAll();
        public void DestroyObjects();
        public void DestroyObjectsToLimit(int limit);
    }
    
    public interface IPool<TPoolObject> : IPool where TPoolObject : IPoolObject
    {
        public event Action<TPoolObject> CreateEvent;
        public event Action<TPoolObject> SpawnEvent;
        public event Action<TPoolObject> ReleaseEvent;
        public IReadOnlyList<TPoolObject> ActiveObjects { get; }
        public TPoolObject Spawn();
        public void Release(TPoolObject instance);
        public void AddInstance(TPoolObject instance, bool isActive, bool isSendCallback);
        public void Prewarm(int value);
    }
}