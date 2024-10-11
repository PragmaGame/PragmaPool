using UnityEngine;

namespace Pragma.Pool
{
    public class PrefabPool<TObject> : Pool<TObject>, IPrefabPool<TObject> where TObject : Component, IPoolObject
    {
        private readonly TObject _prefab;
        
        protected readonly Transform container;

        public Component Prefab => _prefab;

        public PrefabPool(IPoolObjectFactory factory, TObject prefab, Transform parent = null, string name = null) : base(factory)
        {
            _prefab = prefab;
            
            if (string.IsNullOrWhiteSpace(name))
            {
                name = $"Pool: {typeof(TObject).Name}";
            }

            container = new GameObject(name).transform;

            if (parent != null)
            {
                container.SetParent(parent, false);
            }
        }

        protected override object GetCreateData() => _prefab;

        public override TObject Spawn()
        {
            var instance = GetOrCreate();
            
            instance.gameObject.SetActive(true);
            
            RegisterInstance(instance);
            return instance;
        }
        
        public TObject Spawn(Transform parent, bool worldPositionStays = true)
        {
            var instance = GetOrCreate();
            
            instance.transform.SetParent(parent, worldPositionStays);
            instance.gameObject.SetActive(true);
            
            RegisterInstance(instance);
            return instance;
        }
        
        public TObject Spawn(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            var instance = GetOrCreate();
            
            instance.transform.SetParent(parent);
            instance.transform.SetPositionAndRotation(position, rotation);
            instance.gameObject.SetActive(true);
            
            RegisterInstance(instance);
            return instance;
        }

        public override void Release(TObject instance)
        {
            instance.gameObject.SetActive(false);
            instance.transform.SetParent(container);
            
            base.Release(instance);
        }

        protected override void DestroyObject(TObject instance)
        {
            instance.ReleaseRequestAction = null;
            Object.Destroy(instance.gameObject);
        }

        public TComponent SpawnByComponent<TComponent>() where TComponent : Component
        {
            return Spawn().GetComponent<TComponent>();
        }

        public TComponent SpawnByComponent<TComponent>(Transform parent, bool worldPositionStays = true)
            where TComponent : Component
        {
            return Spawn(parent, worldPositionStays).GetComponent<TComponent>();
        }

        public TComponent SpawnByComponent<TComponent>(Vector3 position, Quaternion rotation, Transform parent = null)
            where TComponent : Component
        {
            return Spawn(position, rotation, parent).GetComponent<TComponent>();
        }
    }
}