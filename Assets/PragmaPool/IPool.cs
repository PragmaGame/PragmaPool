using System.Collections.Generic;

namespace Pragma.Pool
{
    public interface IPool
    {
        public void Release(IPoolObject instance);
        public void ReleaseAll();
        public void Clear();
        public void ClearToLimit(int limit);
    }
    
    public interface IPool<TPoolObject> : IPool where TPoolObject : IPoolObject
    {
        public IReadOnlyList<TPoolObject> ActiveObjects { get; }
        public TPoolObject Spawn();
        public void Release(TPoolObject instance);
        public void AddInstance(TPoolObject instance, bool isActive, bool isInvokeCallback);
        public void Prewarm(int value);
    }
}