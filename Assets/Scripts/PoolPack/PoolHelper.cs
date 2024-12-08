using System;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace PoolPack
{
    public static class PoolHelper
    {
        public static ObjectPool<T> CreatePool<T>(PoolManager manager, GameObject prefab, Action<T> onCreate = null) where T: PoolObject
        {
            return new ObjectPool<T>(
                () => Create(prefab, manager, onCreate), 
                (obj) => OnGet(obj, manager), 
                (obj) => OnRelease(obj, manager),    
                OnDestroy, true, manager.PoolSize, manager.PoolSize * 2);
        }

        private static T Create<T>(GameObject prefab, PoolManager poolManager, Action<T> onCreate) where T: PoolObject
        {
            var obj = Object.Instantiate(prefab);
            obj.gameObject.SetActive(false);
            var ret = obj.GetComponent<T>();
            ret.OnCreate(poolManager);
            onCreate?.Invoke(ret);
            return ret;
        }

        private static void OnGet(PoolObject obj, PoolManager poolManager)
        {
            obj.gameObject.SetActive(true);
            obj.OnGet(poolManager.GetRandomPoolData());
            poolManager.ActiveObjects.Add(obj);
        }
        
        private static void OnRelease(PoolObject obj, PoolManager poolManager)
        {
            obj.gameObject.SetActive(false);
            obj.OnRelease();
            poolManager.ActiveObjects.Remove(obj);
        }
        
        private static void OnDestroy(PoolObject obj)
        {
            Object.Destroy(obj.gameObject);
        }
    }
}