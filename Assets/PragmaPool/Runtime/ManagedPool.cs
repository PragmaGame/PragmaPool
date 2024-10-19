using System.Collections.Generic;
using UnityEngine;

namespace Pragma.Pool
{
    public class ManagedPool<TObject> : Pool<TObject>, IManagedPool<TObject> where TObject : class, IPoolObject
    {
        protected readonly List<TObject> activeObjects;
        
        public IReadOnlyList<TObject> ActiveObjects => activeObjects;
        
        public ManagedPool(IPoolObjectFactory factory = null, object createData = null) : base(factory, createData)
        {
            activeObjects = new List<TObject>();
        }

        public bool TryRelease(IPoolObject instance)
        {
            if (instance is TObject convert)
            {
                Release(convert);
                return true;
            }

            return false;
        }
        
        protected void OnReleaseRequest(IPoolObject instance)
        {
            if (!TryRelease(instance))
            {
                Debug.LogError($"Fail release {instance}. Instance has type {instance.GetType()},cannot convert to {typeof(TObject)}");
            }
        }

        protected override TObject Create()
        {
            var instance = base.Create();
            instance.ReleaseRequestAction = OnReleaseRequest;
            return instance;
        }

        protected override void OnDestroy(TObject instance)
        {
            instance.ReleaseRequestAction = null;
        }

        protected override TObject SpawnInternal()
        {
            var instance = base.SpawnInternal();
            activeObjects.Add(instance);
            return instance;
        }

        public override TObject Spawn()
        {
            var instance = SpawnInternal();
            instance.OnSpawn();
            Notify(PoolSignal.Spawn, instance);
            return instance;
        }

        public override void Release(TObject instance)
        {
            activeObjects.Remove(instance);
            instance.OnRelease();
            
            base.Release(instance);
        }
        
        public void Release()
        {
            foreach (var activeObject in activeObjects)
            {
                Release(activeObject);
            }
        }

        public void ReleaseAndDestroy()
        {
            Release();
            Destroy();
        }
        
        public void AddInstance(TObject instance, bool isActive, bool isInvoke = false, bool isNotify = false)
        {
            instance.ReleaseRequestAction = OnReleaseRequest;
            
            if (isActive)
            {
                activeObjects.Add(instance);

                if (isInvoke)
                {
                    instance.OnSpawn();
                }
                
                if (isNotify)
                {
                    Notify(PoolSignal.Spawn, instance);
                }
            }
            else
            {
                sleepObjects.Push(instance);

                if (isInvoke)
                {
                    instance.OnRelease();
                }
                
                if (isNotify)
                {
                    Notify(PoolSignal.Release, instance);
                }
            }
        }
    }
}