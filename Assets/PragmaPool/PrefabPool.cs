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

        protected override TObject Create()
        {
            var instance = factory.Create<TObject>(_prefab);
            instance.ReleaseRequestAction = Release;
            return instance;
        }

        public TComponent Spawn<TComponent>() where TComponent : Component
        {
            return Spawn().gameObject.GetComponent<TComponent>();
        }

        public TComponent Spawn<TComponent>(Transform parent, bool worldPositionStays = true) where TComponent : Component
        {
            throw new System.NotImplementedException();
        }

        public TComponent Spawn<TComponent>(Vector3 position, Quaternion rotation, Transform parent = null) where TComponent : Component
        {
            throw new System.NotImplementedException();
        }

        public override TObject Spawn()
        {
            var instance = GetOrCreate();
            
            instance.gameObject.SetActive(true);
            
            instance.OnSpawn();
            activeObjects.Add(instance);
            
            return instance;
        }
        
        public TObject Spawn(Transform parent, bool worldPositionStays = true)
        {
            var instance = GetOrCreate();
            
            instance.transform.SetParent(parent, worldPositionStays);
            instance.gameObject.SetActive(true);
            
            instance.OnSpawn();
            activeObjects.Add(instance);
            
            return instance;
        }
        
        public TObject Spawn(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            var instance = GetOrCreate();
            
            instance.transform.SetParent(parent);
            instance.transform.SetPositionAndRotation(position, rotation);
            instance.gameObject.SetActive(true);
            
            instance.OnSpawn();
            activeObjects.Add(instance);
            
            return instance;
        }

        public override void Release(TObject instance)
        {
            instance.gameObject.SetActive(false);
            instance.transform.SetParent(container);
            
            base.Release(instance);
        }

        public override void Clear()
        {
            ReleaseAll();

            foreach (var instance in sleepObjects)
            {
                Object.Destroy(instance.gameObject);
            }
            
            sleepObjects.Clear();
        }
    }
}