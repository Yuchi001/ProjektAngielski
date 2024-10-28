using Managers.Base;
using UnityEngine;

namespace Other
{
    public abstract class EntityBase : MonoBehaviour
    {
        public bool Active { get; protected set; }
        
        public void DestroyNonAloc()
        {
            Active = false;
            gameObject.SetActive(false);
        }

        public void SpawnNonAloc()
        {
            Active = true;
            gameObject.SetActive(true);
        }
        
        public void SetBusy()
        {
            Active = true;
        }
        
        public abstract void Setup(SoEntityBase soData);
        public abstract void SpawnSetup(SpawnerBase spawner);

        public T As<T>() where T : EntityBase
        {
            return (T)this;
        }
    }
}