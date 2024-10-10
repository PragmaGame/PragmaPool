using System;

namespace Pragma.Pool
{
    public interface IPoolObject
    {
        public Action<IPoolObject> ReleaseRequestAction { get; set; }
        public void OnSpawn();
        public void OnRelease();

        public virtual void ReleaseRequest()
        {
            ReleaseRequestAction?.Invoke(this);
        }
    }
}