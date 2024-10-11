using System;
using UnityEngine;

namespace Pragma.Pool
{
    public class PrefabPoolObject : MonoBehaviour, IPoolObject
    {
        public event Action SpawnEvent;
        public event Action ReleaseEvent;
        
        public Action<IPoolObject> ReleaseRequestAction { get; set; }
        
        public virtual void OnSpawn()
        {
            SpawnEvent?.Invoke();
        }

        public virtual void OnRelease()
        {
            ReleaseEvent?.Invoke();
        }

        public void ReleaseRequest()
        {
            ReleaseRequestAction?.Invoke(this);
        }
    }
}