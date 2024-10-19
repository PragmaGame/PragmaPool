using System;
using System.Collections.Generic;

namespace Pragma.Pool
{
    public static class ManagedPoolExtension
    {
        public static List<TObject> FilterActiveObjects<TObject>(this IManagedPool<TObject> pool, Predicate<TObject> condition) where TObject : class, IPoolObject
        {
            var result = new List<TObject>();

            FilterActiveObjects(pool, result, condition);

            return result;
        }
        
        public static void FilterActiveObjects<TObject>(this IManagedPool<TObject> pool, List<TObject> result, Predicate<TObject> condition) where TObject : class, IPoolObject
        {
            foreach (var activeObject in pool.ActiveObjects)
            {
                if (condition.Invoke(activeObject))
                {
                    result.Add(activeObject);
                }
            }
        }
    }
}