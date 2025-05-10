using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace PoolPack
{
    public static class PoolHelper
    {
        public static ObjectPool<T> CreatePool<T>(PoolManager manager, T prefab, bool randomGet) where T: PoolObject
        {
            var pool = new ObjectPool<T>(
                () => Create<T>(prefab, manager), 
                (obj) => OnGet(obj, manager, randomGet), 
                (obj) => OnRelease(obj, manager),    
                OnDestroy, true, manager.PoolSize, manager.PoolSize * 2);
            for (var i = 0; i < manager.PoolSize; i++)
            {
                var obj = pool.Get();
                pool.Release(obj);
            }

            return pool;
        }

        private static T Create<T>(T prefab, PoolManager poolManager) where T: PoolObject
        {
            var obj = Object.Instantiate(prefab, poolManager.transform);
            var ret = obj.GetComponent<T>();
            ret.OnCreate(poolManager);
            ret.SetActive(true);
            return ret;
        }

        private static void OnGet(PoolObject obj, PoolManager poolManager, bool randomGet)
        {
            obj.SetActive(true);
            if (randomGet) obj.OnGet(poolManager.GetRandomPoolData());
            poolManager.ActiveObjects.Add(obj);
        }
        
        private static void OnRelease(PoolObject obj, PoolManager poolManager)
        {
            if (!obj.Active) return;
            
            obj.SetActive(false);
            obj.OnRelease();
            poolManager.ActiveObjects.Remove(obj);
        }
        
        private static void OnDestroy(PoolObject obj)
        {
            Object.Destroy(obj.gameObject);
        }
    }
}