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
        public TComponent Spawn<TComponent>() where TComponent : Component;
        public TComponent Spawn<TComponent>(Transform parent, bool worldPositionStays = true) where TComponent : Component;
        public TComponent Spawn<TComponent>(Vector3 position, Quaternion rotation, Transform parent = null) where TComponent : Component;
    }
}