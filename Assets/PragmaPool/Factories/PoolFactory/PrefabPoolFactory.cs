using UnityEngine;

namespace Pragma.Pool
{
    public class PrefabPoolFactory : IPrefabPoolFactory
    {
        private readonly IPoolObjectFactory _objectFactory;
        
        public PrefabPoolFactory()
        {
            _objectFactory = new PrefabPoolObjectFactory();
        }
        
        public IPrefabPool<TPoolObject> Create<TPoolObject>(TPoolObject prefab, Transform parent) where TPoolObject : Component, IPoolObject
        {
            return new PrefabPool<TPoolObject>(_objectFactory, prefab, parent);
        }
    }
}