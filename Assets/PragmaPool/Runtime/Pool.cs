using System;
using System.Collections.Generic;

namespace Pragma.Pool
{
    public class Pool<TObject> : IPool<TObject> where TObject : class
    {
        protected readonly IPoolObjectFactory factory;
        protected readonly List<TObject> activeObjects;
        protected readonly Stack<TObject> sleepObjects;

        protected object createData;
        
        public event Action<TObject> CreateEvent;
        public event Action<TObject> DestroyEvent;
        public event Action<TObject> SpawnEvent;
        public event Action<TObject> ReleaseEvent;
        
        public IReadOnlyList<TObject> ActiveObjects => activeObjects;

        public Pool(IPoolObjectFactory factory = null, object createData = null)
        {
            this.createData = createData;
            this.factory = factory ?? new ActivatorPoolObjectFactory();

            activeObjects = new List<TObject>();
            sleepObjects = new Stack<TObject>();
        }

        protected TObject Create()
        {
            var instance = factory.Create<TObject>(createData);
            OnCreate(instance);
            CreateEvent?.Invoke(instance);
            return instance;
        }

        protected virtual void OnCreate(TObject instance)
        {
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

        protected void RegisterInstance(TObject instance, bool isSendCallback = true)
        {
            OnRegisterInstance(instance);
            
            if (isSendCallback)
            {
                SpawnEvent?.Invoke(instance);   
            }

            activeObjects.Add(instance);
        }

        protected virtual void OnRegisterInstance(TObject instance)
        {
        }
        
        protected void DeregisterInstance(TObject instance, bool isSendCallback = true)
        {
            OnDeregisterInstance(instance);
            
            if (isSendCallback)
            {
                ReleaseEvent?.Invoke(instance);
            }

            activeObjects.Remove(instance);
            sleepObjects.Push(instance);
        }

        protected virtual void OnDeregisterInstance(TObject instance)
        {
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

        protected void Destroy(TObject instance)
        {
            DestroyEvent?.Invoke(instance);
            
            OnDestroy(instance);

            if (instance is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        protected virtual void OnDestroy(TObject instance)
        {
        }

        public void AddInstance(TObject instance, bool isActive, bool isSendCallback)
        {
            if (isActive)
            {
                RegisterInstance(instance, isSendCallback);
            }
            else
            {
                DeregisterInstance(instance, isSendCallback);
            }
            
            OnAddInstance(instance);
        }

        protected virtual void OnAddInstance(TObject instance)
        {
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