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
        
        public virtual void OnCreate(PoolManager poolManager)
        {
            Active = false;
        }
        
        public virtual void OnGet(SoEntityBase so)
        {
            Active = true;
        }
        
        public virtual void OnRelease()
        {
            Active = false;
        }

        protected void SetTimer(string ID)
        {
            if (_lastMeasure.TryAdd(ID, DateTime.Now)) return;
            
            _lastMeasure[ID] = DateTime.Now;
        }

        protected double CheckTimer(string ID)
        {
            var hasValue = _lastMeasure.TryGetValue(ID, out var lastMeasure);
            return hasValue ? (DateTime.Now - lastMeasure).TotalSeconds : 0;
        }

        public virtual void InvokeUpdate()
        {
            
        }
    }
}