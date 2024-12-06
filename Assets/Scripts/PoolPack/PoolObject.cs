using Other;
using UnityEngine;

namespace PoolPack
{
    public abstract class PoolObject : MonoBehaviour
    {
        protected bool Active { get; private set; }
        
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

        public virtual void InvokeUpdate()
        {
            
        }
    }
}