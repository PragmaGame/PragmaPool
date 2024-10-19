using System;
using System.Collections.Generic;

namespace Pragma.Pool
{
    public class Pool<TObject> : IPool<TObject> where TObject : class
    {
        private readonly Dictionary<PoolSignal, Action<TObject>> _notifyActions;
        
        protected readonly IPoolObjectFactory factory;
        protected readonly Stack<TObject> sleepObjects;

        protected object createData;

        public Pool(IPoolObjectFactory factory = null, object createData = null)
        {
            this.createData = createData;
            this.factory = factory ?? new ActivatorPoolObjectFactory();
            
            sleepObjects = new Stack<TObject>();

            _notifyActions = new Dictionary<PoolSignal, Action<TObject>>
            {
                { PoolSignal.Create, default },
                { PoolSignal.Spawn, default },
                { PoolSignal.Release, default },
                { PoolSignal.Destroy, default }
            };
        }

        public void Register(PoolSignal signal, Action<TObject> handler)
        {
            _notifyActions[signal] += handler;
        }

        public void Deregister(PoolSignal signal, Action<TObject> handler)
        {
            _notifyActions[signal] -= handler;
        }

        protected void Notify(PoolSignal signal, TObject instance)
        {
            _notifyActions[signal]?.Invoke(instance);
        }
        
        protected virtual TObject Create()
        {
            var instance = factory.Create<TObject>(createData);
            Notify(PoolSignal.Create, instance);
            return instance;
        }

        protected virtual TObject SpawnInternal()
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
            var instance = SpawnInternal();
            Notify(PoolSignal.Spawn, instance);
            return instance;
        }

        public virtual void Release(TObject instance)
        {
            sleepObjects.Push(instance);
            Notify(PoolSignal.Release, instance);
        }

        protected void Destroy(TObject instance)
        {
            Notify(PoolSignal.Destroy, instance);
            
            OnDestroy(instance);

            if (instance is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        protected virtual void OnDestroy(TObject instance)
        {
        }

        public virtual void Prewarm(int value)
        {
            for (var i = 0; i < value; i++)
            {
                sleepObjects.Push(Create());
            }
        }

        public void Destroy(int remainder = 0)
        {
            var different = sleepObjects.Count - remainder;

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