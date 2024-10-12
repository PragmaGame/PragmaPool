namespace Pragma.Pool
{
    public interface IPoolObjectFactory
    {
        public T Create<T>(object data = null) where T : class;
    }
}