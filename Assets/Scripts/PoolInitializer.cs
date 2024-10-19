using System;
using Pragma.Pool;
using UnityEngine;

namespace DefaultNamespace
{
    public class PoolInitializer : MonoBehaviour
    {
        [SerializeField] private Transform _container;
        [SerializeField] private APoolObject _aPoolObjectPrefab;
        [SerializeField] private APoolObject _a1PoolObjectPrefab;
        [SerializeField] private TestComponent _test1ComponentPrefab;
        [SerializeField] private TestComponent _test2ComponentPrefab;
        
        [SerializeField] private TestPoolObject _testPoolObjectPrefab;

        private PrefabPoolAggregator _prefabPoolAggregator;

        private void Awake()
        {
            _prefabPoolAggregator = new PrefabPoolAggregator(new PrefabPoolFactory(), _container);
        }

        private void Start()
        {
            var pool = _prefabPoolAggregator.GetPool(_a1PoolObjectPrefab);
            pool.Deregister(PoolSignal.Release, OnRelease);
            pool.Register(PoolSignal.Release, OnRelease);
            
            var a = _prefabPoolAggregator.Spawn(_aPoolObjectPrefab);
            var a1 = _prefabPoolAggregator.Spawn(_a1PoolObjectPrefab);
            var a11= _prefabPoolAggregator.Spawn(_a1PoolObjectPrefab);
            
            var test1Component = _prefabPoolAggregator.SpawnByComponent<TestComponent>(_test1ComponentPrefab);
            var test2Component = _prefabPoolAggregator.SpawnByComponent<TestComponent>(_test2ComponentPrefab);
            
            _prefabPoolAggregator.Release(a);
            _prefabPoolAggregator.Release(a1);
            _prefabPoolAggregator.Release(a11);
            
            _prefabPoolAggregator.Release(test1Component);
            _prefabPoolAggregator.Release(test2Component);
        }

        private void OnRelease(APoolObject aPoolObject)
        {
            Debug.Log($"Release {aPoolObject}", aPoolObject);
        }
    }
}