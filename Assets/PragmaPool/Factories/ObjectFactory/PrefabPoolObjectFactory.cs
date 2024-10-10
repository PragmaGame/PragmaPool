using UnityEngine;

namespace Pragma.Pool
{
    public class PrefabPoolObjectFactory : IPoolObjectFactory
    {
        public T Create<T>(object data) where T : class
        {
            return Object.Instantiate(data as Object) as T;
        }
    }
}