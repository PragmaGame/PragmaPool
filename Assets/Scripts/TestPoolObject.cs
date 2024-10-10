using System;
using Pragma.Pool;

namespace DefaultNamespace
{
    public class TestPoolObject : IPoolObject
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