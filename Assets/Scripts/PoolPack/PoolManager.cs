using System.Collections.Generic;
using Other;
using UnityEngine;

namespace PoolPack
{
    public abstract class PoolManager : MonoBehaviour
    {
        [SerializeField] private float maxUpdateStackDuration = 0.5f;
        [field: SerializeField, Range(10, 1000)] public int PoolSize { get; protected set; }
        public List<PoolObject> ActiveObjects { get; } = new();
        

        public abstract PoolObject GetPoolObject();
        public abstract void ReleasePoolObject(PoolObject poolObject);
        public abstract SoEntityBase GetRandomPoolData();

        
        private Stack<PoolObject> updateStack = new();
        private float _currentQueueLength = 1;
        
        protected void PrepareQueue()
        {
            updateStack = new Stack<PoolObject>(ActiveObjects);
        }

        protected void RunUpdatePoolStack()
        {
            if (updateStack.Count == 0) PrepareQueue();

            var fps = 1.0f / Time.smoothDeltaTime;
            
            _currentQueueLength = Mathf.CeilToInt(updateStack.Count / (fps * maxUpdateStackDuration));
            for (var i = 0; i < _currentQueueLength && updateStack.Count > 0; i++)
            {
                updateStack.Pop().InvokeUpdate();
            }
        }
    }
}