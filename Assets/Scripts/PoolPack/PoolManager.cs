using System;
using System.Collections.Generic;
using Other;
using ProjectilePack;
using UnityEngine;

namespace PoolPack
{
    public abstract class PoolManager : MonoBehaviour
    {
        [SerializeField] protected float maxUpdateStackDuration = 0.5f;
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
        
        protected void PrepareQueue()
        {
            updateStack = new Stack<PoolObject>(ActiveObjects);
        }

        protected void RunUpdatePoolStack()
        {
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
            toRet.InvokeUpdate();
            return toRet;
        }
    }
}