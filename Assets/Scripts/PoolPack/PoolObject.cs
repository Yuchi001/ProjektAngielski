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
        /// Do not call base method
        /// </summary>
        public virtual void InvokeUpdate()
        {
            
        }
    }
}