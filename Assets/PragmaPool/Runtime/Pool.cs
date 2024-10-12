using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pragma.Pool
{
    public class Pool<TObject> : IPool<TObject> where TObject : class, IPoolObject
    {
        protected readonly IPoolObjectFactory factory;
        protected readonly List<TObject> activeObjects;
        protected readonly Stack<TObject> sleepObjects;

        public event Action<TObject> CreateEvent;
        public event Action<TObject> DestroyEvent;
        public event Action<TObject> SpawnEvent;
        public event Action<TObject> ReleaseEvent;
        
        public IReadOnlyList<TObject> ActiveObjects => activeObjects;

        public Pool(IPoolObjectFactory factory = null)
        {
            this.factory = factory ?? new ActivatorPoolObjectFactory();

            activeObjects = new List<TObject>();
            sleepObjects = new Stack<TObject>();
        }

        protected virtual object GetCreateData() => null;

        protected virtual TObject Create()
        {
            var instance = factory.Create<TObject>(GetCreateData());
            instance.ReleaseRequestAction = Release;
            CreateEvent?.Invoke(instance);
            return instance;
        }
        
        protected TObject GetOrCreate()
        {
            TObject instance;
            
            if (sleepObjects.Count > 0)
            {
                instance = sleepObjects.Pop();
            }
            else
            {
                instance = Create();
            }

            return instance;
        }

        public virtual TObject Spawn()
        {
            var instance = GetOrCreate();
            RegisterInstance(instance);
            return instance;
        }

        protected virtual void RegisterInstance(TObject instance, bool isSendCallback = true)
        {
            if (isSendCallback)
            {
                instance.OnSpawn();
                SpawnEvent?.Invoke(instance);   
            }

            activeObjects.Add(instance);
        }
        
        protected virtual void DeregisterInstance(TObject instance, bool isSendCallback = true)
        {
            if (isSendCallback)
            {
                instance.OnRelease();
                ReleaseEvent?.Invoke(instance);
            }

            activeObjects.Remove(instance);
            sleepObjects.Push(instance);
        }

        public void Release(IPoolObject instance)
        {
            if (instance is TObject convert)
            {
                Release(convert);
            }
            else
            {
                Debug.LogError($"Fail release {instance}. Instance has type {instance.GetType()},cannot convert to {typeof(TObject)}");
            }
        }

        public virtual void Release(TObject instance)
        {
            DeregisterInstance(instance);
        }

        public void ReleaseAll()
        {
            foreach (var activeObject in activeObjects)
            {
                Release(activeObject);
            }
        }
        
        public virtual void DestroyAll()
        {
            ReleaseAll();
            
            foreach (var sleepObject in sleepObjects)
            {
                Destroy(sleepObject);
            }
            
            sleepObjects.Clear();
        }

        protected virtual void Destroy(TObject instance)
        {
            DestroyEvent?.Invoke(instance);
            
            instance.ReleaseRequestAction = null;

            if (instance is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        public void AddInstance(TObject instance, bool isActive, bool isSendCallback)
        {
            if (isActive)
            {
                instance.ReleaseRequestAction = Release;

                RegisterInstance(instance, isSendCallback);
            }
            else
            {
                DeregisterInstance(instance, isSendCallback);
            }
        }

        public virtual void Prewarm(int value)
        {
            for (var i = 0; i < value; i++)
            {
                sleepObjects.Push(Create());
            }
        }

        public void DestroyToLimit(int limit)
        {
            var different = sleepObjects.Count - limit;

            if (different <= 0)
            {
                return;
            }

            for (var i = 0; i < different; i++)
            {
                Destroy(sleepObjects.Pop());
            }
        }
    }
}