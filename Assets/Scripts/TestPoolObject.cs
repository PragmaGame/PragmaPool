using System;
using Pragma.Pool;
using UnityEngine;

namespace DefaultNamespace
{
    public class TestPoolObject : MonoBehaviour, IPoolObject
    {
        public Action<IPoolObject> ReleaseRequestAction { get; set; }
        
        public void OnSpawn()
        {
            throw new NotImplementedException();
        }

        public void OnRelease()
        {
        }
    }
}