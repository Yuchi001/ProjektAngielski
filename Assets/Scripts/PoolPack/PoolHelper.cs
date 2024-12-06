using Other;
using UnityEngine;
using UnityEngine.Pool;

namespace PoolPack
{
    public static class PoolHelper
    {
        public static ObjectPool<T> CreatePool<T>(PoolManager manager, GameObject prefab, int size) where T: PoolObject
        {
            return new ObjectPool<T>(
                () => Create<T>(prefab, manager), 
                (obj) => OnGet<T>(obj, manager), 
                (obj) => OnRelease(obj, manager),    
                OnDestroy, true, size, size * 2);
        }

        private static T Create<T>(GameObject prefab, PoolManager poolManager) where T: PoolObject
        {
            var obj = Object.Instantiate(prefab);
            obj.gameObject.SetActive(false);
            var ret = obj.GetComponent<T>();
            ret.OnCreate(poolManager);
            return ret;
        }

        private static void OnGet<T>(PoolObject obj, PoolManager poolManager) where T: PoolObject
        {
            obj.gameObject.SetActive(true);
            obj.OnGet(poolManager.GetRandomPoolData());
            poolManager.ActiveObjects.Add(obj);
        }
        
        private static void OnRelease(PoolObject obj, PoolManager poolManager)
        {
            obj.gameObject.SetActive(false);
            obj.OnRelease();
            poolManager.ActiveObjects.Add(obj);
        }
        
        private static void OnDestroy(PoolObject obj)
        {
            Object.Destroy(obj.gameObject);
        }
    }
}