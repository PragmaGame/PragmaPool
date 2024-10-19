using UnityEngine;

namespace Pragma.Pool
{
    public interface IPrefabPool : IManagedPool
    {
        public Component Prefab { get; }
    }
    
    public interface IPrefabPool<TPoolObject> : IPrefabPool, IManagedPool<TPoolObject> where TPoolObject : Component, IPoolObject
    {
        public TPoolObject Spawn(Transform parent, bool worldPositionStays = true);
        public TPoolObject Spawn(Vector3 position, Quaternion rotation, Transform parent = null);
    }
}