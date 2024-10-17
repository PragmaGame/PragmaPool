using System;
using System.Collections.Generic;

namespace Pragma.Pool
{
    public interface IPool
    {
        public void ReleaseAll();
        public void DestroyAll();
        public void DestroyToLimit(int limit);
    }
    
    public interface IPool<TObject> : IPool where TObject : class
    {
        public event Action<TObject> CreateEvent;
        public event Action<TObject> DestroyEvent;
        public event Action<TObject> SpawnEvent;
        public event Action<TObject> ReleaseEvent;
        public IReadOnlyList<TObject> ActiveObjects { get; }
        public TObject Spawn();
        public void Release(TObject instance);
        public void AddInstance(TObject instance, bool isActive, bool isSendCallback);
        public void Prewarm(int value);
    }
}