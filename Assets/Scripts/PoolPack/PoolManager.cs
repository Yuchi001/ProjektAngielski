using System;
using System.Collections.Generic;
using Other;
using ProjectilePack;
using UnityEngine;
using UnityEngine.Pool;

namespace PoolPack
{
    public abstract class PoolManager : MonoBehaviour
    {
        [SerializeField] private bool usesUpdateStack;
        [SerializeField] protected float maxUpdateStackDuration = 0.02f;
        [field: SerializeField, Range(10, 1000)] public int PoolSize { get; protected set; }
        public List<PoolObject> ActiveObjects { get; } = new();
        protected abstract T GetPoolObject<T>() where T: PoolObject;
        public abstract void ReleasePoolObject(PoolObject poolObject);
        
        public virtual SoPoolObject GetRandomPoolData()
        {
            return null;
        }
        
        private Stack<PoolObject> updateStack = new();
        private float _currentQueueLength = 1;
        
        private void PrepareQueue()
        {
            updateStack = new Stack<PoolObject>(ActiveObjects);
        }

        protected virtual void Update()
        {
            if (!usesUpdateStack) return;
            
            if (updateStack.Count == 0) PrepareQueue();
            
            var fps = 1.0f / Time.unscaledDeltaTime;
            var requiredRate = updateStack.Count / maxUpdateStackDuration;
            _currentQueueLength = Mathf.Min(updateStack.Count, Mathf.CeilToInt(requiredRate / fps));
            for (var i = 0; i < _currentQueueLength && updateStack.Count > 0; i++)
            {
                InvokeQueueUpdate();
            }
        }

        protected virtual PoolObject InvokeQueueUpdate()
        {
            var toRet = updateStack.Pop();
            toRet.InvokeFixedUpdate();
            return toRet;
        }
        
        protected void ClearAll<T>(ObjectPool<T> pool) where T: PoolObject
        {
            foreach (var obj in ActiveObjects)
            {
                if (obj != null && obj.gameObject.activeInHierarchy)
                    Destroy(obj.gameObject); 
            }
            ActiveObjects.Clear();
            pool.Clear();
        }
    }
}