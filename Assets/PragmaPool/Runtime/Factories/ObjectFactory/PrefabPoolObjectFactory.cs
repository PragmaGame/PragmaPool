using UnityEngine;

namespace Pragma.Pool
{
    public class PrefabPoolObjectFactory : IPoolObjectFactory
    {
        public T Create<T>(object data) where T : class
        {
            var poolObject = Object.Instantiate(data as Component);
            
            //SceneManager.MoveGameObjectToScene(poolObject.gameObject, SceneManager.GetActiveScene());

            return poolObject as T;
        }
    }
}