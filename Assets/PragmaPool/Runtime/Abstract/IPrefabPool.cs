using UnityEngine;

namespace Pragma.Pool
{
    public interface IPrefabPool : ISendingPool
    {
        public Component Prefab { get; }
    }
    
    public interface IPrefabPool<TPoolObject> : IPrefabPool, ISendingPool<TPoolObject> where TPoolObject : Component, IPoolObject
    {
        public TPoolObject Spawn(Transform parent, bool worldPositionStays = true);
        public TPoolObject Spawn(Vector3 position, Quaternion rotation, Transform parent = null);
    }
}