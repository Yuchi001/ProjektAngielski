using System.Collections.Generic;
using System.Linq;
using PoolPack;
using UnityEngine;

namespace EnemyPack
{
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] private float maxUpdateStackDuration = 0.5f;
        
        private List<PoolObject> enemies = new();
        private Stack<PoolObject> updateStack = new();

        private float _currentQueueLength = 1;
        
        public void Setup(List<PoolObject> enemies)
        {
            this.enemies = enemies;
            PrepareQueue();
        }

        private void PrepareQueue()
        {
            var validEnemies = enemies.Where(e => e != default && e.isActiveAndEnabled);
            updateStack = new Stack<PoolObject>(validEnemies);
        }

        private void Update()
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