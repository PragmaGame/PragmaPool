using System.Collections.Generic;
using UnityEngine;

namespace Pragma.Pool
{
    public class Pool<TObject> : IPool<TObject> where TObject : class, IPoolObject
    {
        protected readonly IPoolObjectFactory factory;
        protected readonly List<TObject> activeObjects;
        protected readonly Stack<TObject> sleepObjects;

        public IReadOnlyList<TObject> ActiveObjects => activeObjects;

        public Pool(IPoolObjectFactory factory = null)
        {
            this.factory = factory ?? new ActivatorPoolObjectFactory();

            activeObjects = new List<TObject>();
            sleepObjects = new Stack<TObject>();
        }

        protected virtual TObject Create()
        {
            var instance = factory.Create<TObject>();
            instance.ReleaseRequestAction = Release;
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
            instance.OnSpawn();
            activeObjects.Add(instance);
            return instance;
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
            instance.OnRelease();
            activeObjects.Remove(instance);
            sleepObjects.Push(instance);
        }

        public void ReleaseAll()
        {
            foreach (var activeObject in activeObjects)
            {
                Release(activeObject);
            }
        }
        
        public virtual void Clear()
        {
            ReleaseAll();
            sleepObjects.Clear();
        }

        public void AddInstance(TObject instance, bool isActive, bool isInvokeCallback)
        {
            if (isActive)
            {
                instance.ReleaseRequestAction = Release;

                if (isInvokeCallback)
                {
                    instance.OnSpawn();
                }
                
                activeObjects.Add(instance);
            }
            else
            {
                if (isInvokeCallback)
                {
                    instance.OnRelease();
                }
                
                sleepObjects.Push(instance);
            }
        }

        public virtual void Prewarm(int value)
        {
            for (var i = 0; i < value; i++)
            {
                sleepObjects.Push(Create());
            }
        }

        public void ClearToLimit(int limit)
        {
            var different = activeObjects.Count - limit;

            if (different <= 0)
            {
                return;
            }

            for (var i = 0; i < different; i++)
            {
                sleepObjects.Pop();
            }
        }
    }
}