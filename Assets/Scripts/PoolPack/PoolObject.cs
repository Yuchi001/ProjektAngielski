using System;
using System.Collections.Generic;
using Other;
using UnityEngine;

namespace PoolPack
{
    public abstract class PoolObject : MonoBehaviour
    {
        protected bool Active { get; private set; }
        private readonly Dictionary<string, DateTime> _lastMeasure = new();
        private DateTime _lastUpdated;
        
        protected float deltaTime { get; private set; }

        public T As<T>() where T: PoolObject
        {
            return (T)this;
        }
        
        public virtual void OnCreate(PoolManager poolManager)
        {
            Active = false;
        }
        
        public virtual void OnGet(SoPoolObject so)
        {
            Active = true;
            _lastUpdated = DateTime.Now;
        }
        
        public virtual void OnRelease()
        {
            Active = false;
            
            _lastMeasure.Clear();
        }

        public void SetTimer(string ID)
        {
            if (_lastMeasure.TryAdd(ID, DateTime.Now)) return;
            
            _lastMeasure[ID] = DateTime.Now;
        }

        public double CheckTimer(string ID)
        {
            var hasValue = _lastMeasure.TryGetValue(ID, out var lastMeasure);
            return hasValue ? (DateTime.Now - lastMeasure).TotalSeconds : 0;
        }

        /// <summary>
        /// Base method contains deltaTime calculation
        /// </summary>
        public virtual void InvokeUpdate()
        {
            var now = DateTime.Now;
            deltaTime = (float)(now - _lastUpdated).TotalSeconds;
            _lastUpdated = now;
        }
    }
}