using System;

namespace Pragma.Pool
{
    public class ActivatorPoolObjectFactory : IPoolObjectFactory
    {
        public T Create<T>(object data) where T : class
        {
            return Activator.CreateInstance<T>();
        }
    }
}