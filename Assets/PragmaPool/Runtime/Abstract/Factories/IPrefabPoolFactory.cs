using UnityEngine;

namespace Pragma.Pool
{
    public interface IPrefabPoolFactory
    {
        public IPrefabPool<TPoolObject> Create<TPoolObject>(TPoolObject prefab, Transform parent) where TPoolObject : Component, IPoolObject;
    }
}