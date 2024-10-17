namespace Pragma.Pool
{
    public interface ISendingPool : IPool
    {
        public bool TryRelease(IPoolObject instance);
    }
    
    public interface ISendingPool<TPoolObject> : ISendingPool, IPool<TPoolObject> where TPoolObject : class, IPoolObject
    {
    }
}