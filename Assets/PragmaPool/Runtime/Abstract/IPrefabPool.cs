using UnityEngine;

namespace Pragma.Pool
{
    public interface IPrefabPool : IPool
    {
        public Component Prefab { get; }
    }
    
    public interface IPrefabPool<TPoolObject> : IPrefabPool, IPool<TPoolObject> where TPoolObject : Component, IPoolObject
    {
        public TPoolObject Spawn(Transform parent, bool worldPositionStays = true);
        public TPoolObject Spawn(Vector3 position, Quaternion rotation, Transform parent = null);
    }
}