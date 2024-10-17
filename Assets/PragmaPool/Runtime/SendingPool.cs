using UnityEngine;

namespace Pragma.Pool
{
    public class SendingPool<TObject> : Pool<TObject> where TObject : class, IPoolObject
    {
        public SendingPool(IPoolObjectFactory factory = null, object createData = null) : base(factory, createData)
        {
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
        
        protected void OnRelease(IPoolObject instance)
        {
            if (!TryRelease(instance))
            {
                Debug.LogError($"Fail release {instance}. Instance has type {instance.GetType()},cannot convert to {typeof(TObject)}");
            }
        }

        protected override void OnCreate(TObject instance)
        {
            instance.ReleaseRequestAction = OnRelease;
        }

        protected override void OnAddInstance(TObject instance)
        {
            instance.ReleaseRequestAction = OnRelease;
        }
        
        protected override void OnDestroy(TObject instance)
        {
            instance.ReleaseRequestAction = null;
        }

        protected override void OnRegisterInstance(TObject instance)
        {
            instance.OnSpawn();
        }

        protected override void OnDeregisterInstance(TObject instance)
        {
            instance.OnRelease();
        }
    }
}