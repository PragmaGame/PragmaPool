using System;

namespace Pragma.Pool
{
    public interface IPool
    {
        public void Destroy(int remainder = 0);
        public void Prewarm(int value);
    }
    
    public interface IPool<TObject> : IPool where TObject : class
    {
        public void Register(PoolSignal signal, Action<TObject> handler);
        public void Deregister(PoolSignal signal, Action<TObject> handler);
        public TObject Spawn();
        public void Release(TObject instance);
    }
}