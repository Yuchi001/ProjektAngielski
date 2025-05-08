using System;
using System.Collections.Generic;
using Other;
using UnityEngine;

namespace PoolPack
{
    public abstract class PoolObject : MonoBehaviour
    {
        public bool Active { get; private set; }
        private readonly Dictionary<string, float> _lastMeasure = new();
        private float _lastUpdatedTime;
        
        public float deltaTime { get; private set; }

        public T As<T>() where T: PoolObject
        {
            return (T)this;
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
            Active = active;
        }
        
        public virtual void OnCreate(PoolManager poolManager)
        {
            
        }
        
        public virtual void OnGet(SoPoolObject so)
        {
            deltaTime = Time.deltaTime;
            _lastUpdatedTime = Time.time;
        }
        
        public virtual void OnRelease()
        {
            _lastMeasure.Clear();
        }

        public void SetTimer(string ID)
        {
            if (_lastMeasure.TryAdd(ID, Time.time)) return;

            _lastMeasure[ID] = Time.time;
        }

        public float CheckTimer(string ID)
        {
            var hasValue = _lastMeasure.TryGetValue(ID, out var lastMeasure);
            return hasValue ? Time.time - lastMeasure : 0f;
        }

        /// <summary>
        /// Base method contains deltaTime calculation
        /// </summary>
        public virtual void InvokeUpdate()
        {
            var now = Time.time;
            deltaTime = now - _lastUpdatedTime;
            _lastUpdatedTime = now;
        }
    }
}