using System.Collections.Generic;
using UnityEngine;

namespace Pragma.Pool
{
    public class PrefabPoolAggregator
    {
        private readonly IPrefabPoolFactory _factory;
        private readonly Dictionary<Component, IPrefabPool> _pools = new();
        private readonly Transform _container;

        public PrefabPoolAggregator(IPrefabPoolFactory factory, Transform parent = null, string name = null)
        {
            _factory = factory;

            if (string.IsNullOrWhiteSpace(name))
            {
                name = $"Prefab Pool Aggregator [{factory.GetType().Name}]";
            }
            
            _container = new GameObject(name).transform;
            _container.SetParent(parent);
        }

        public IPrefabPool<TPoolObject> GetPool<TPoolObject>(TPoolObject prefab) where TPoolObject : Component, IPoolObject
        {
            if (_pools.TryGetValue(prefab, out var pool))
            {
                return (IPrefabPool<TPoolObject>)pool;
            }

            var prefabPool = _factory.Create(prefab, _container);
            _pools.Add(prefab, prefabPool);

            return prefabPool;
        }

        public TPoolObject Spawn<TPoolObject>(TPoolObject prefab) where TPoolObject : Component, IPoolObject
        {
            return GetPool(prefab).Spawn();
        }
        
        public TPoolObject Spawn<TPoolObject>(TPoolObject prefab, Transform parent, bool worldPositionStay) where TPoolObject : Component, IPoolObject
        {
            return GetPool(prefab).Spawn(parent, worldPositionStay);
        }
        
        public TPoolObject Spawn<TPoolObject>(TPoolObject prefab, Vector3 position, Quaternion rotation, Transform parent = null) where TPoolObject : Component, IPoolObject
        {
            return GetPool(prefab).Spawn(position, rotation, parent);
        }
        
        public TComponent Spawn<TComponent>(GameObject prefab) where TComponent : Component
        {
            var origin = prefab.GetComponent<PrefabPoolObject>();
            return GetPool(origin).Spawn().GetComponent<TComponent>();
        }

        public void Release<TPoolObject>(TPoolObject instance) where TPoolObject : Component, IPoolObject
        {
            instance.ReleaseRequest();
        }
        
        public void Release(Component instance)
        {
            instance.GetComponent<PrefabPoolObject>().ReleaseRequest();
        }

        public void ReleasePool<TPoolObject>(TPoolObject prefab) where TPoolObject : Component, IPoolObject
        {
            if (_pools.TryGetValue(prefab, out var pool))
            {
                pool.ReleaseAll();
            }
        }

        public void ReleaseAllPools()
        {
            foreach (var pool in _pools.Values)
            {
                pool.ReleaseAll();
            }
        }

        public void DestroyObjectsInPool<TPoolObject>(TPoolObject prefab) where TPoolObject : Component, IPoolObject
        {
            if (_pools.TryGetValue(prefab, out var pool))
            {
                pool.DestroyAll();
            }
        }

        public void DestroyObjectsInAllPolls()
        {
            foreach (var pool in _pools.Values)
            {
                pool.DestroyAll();
            }
        }

        public void Prewarm<TPoolObject>(TPoolObject prefab, int value) where TPoolObject : Component, IPoolObject
        {
            GetPool(prefab).Prewarm(value);
        }
    }
}
