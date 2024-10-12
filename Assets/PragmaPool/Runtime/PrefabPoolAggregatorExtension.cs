using UnityEngine;

namespace Pragma.Pool
{
    public static class PrefabPoolAggregatorExtension
    {
        public static TComponent SpawnByComponent<TComponent>(this PrefabPoolAggregator aggregator, Component prefab) where TComponent : Component
        {
            var origin = prefab.GetComponent<PrefabPoolObject>();
            return SpawnByComponent<TComponent>(aggregator, origin);
        }
        
        public static TComponent SpawnByComponent<TComponent>(this PrefabPoolAggregator aggregator, GameObject prefab) where TComponent : Component
        {
            var origin = prefab.GetComponent<PrefabPoolObject>();
            return SpawnByComponent<TComponent>(aggregator, origin);
        }

        public static TComponent SpawnByComponent<TComponent>(this PrefabPoolAggregator aggregator, PrefabPoolObject prefab) where TComponent : Component
        {
            return aggregator.GetPool(prefab).Spawn().GetComponent<TComponent>();
        }
    }
}