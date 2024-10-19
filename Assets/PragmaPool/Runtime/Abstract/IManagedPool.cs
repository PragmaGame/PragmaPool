using System.Collections.Generic;

namespace Pragma.Pool
{
    public interface IManagedPool : IPool
    {
        public void Release();
        public void ReleaseAndDestroy();
        public bool TryRelease(IPoolObject instance);
    }
    
    public interface IManagedPool<TPoolObject> : IManagedPool, IPool<TPoolObject> where TPoolObject : class, IPoolObject
    {
        public IReadOnlyList<TPoolObject> ActiveObjects { get; }
        public void AddInstance(TPoolObject instance, bool isActive, bool isInvoke = false, bool isNotify = false);
    }
}