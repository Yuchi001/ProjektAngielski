using PlayerPack;
using PoolPack;
using UnityEngine;

namespace InventoryPack.WorldItemPack
{
    public class ReleaseState : IWorldItemState
    {
        private readonly float _releaseTime;
        private readonly PoolManager _poolManager;

        private float _timer = 0;
        
        public ReleaseState(float releaseTime, PoolManager poolManager)
        {
            _releaseTime = releaseTime;
            _poolManager = poolManager;
        }
        
        public void Enter(WorldItem item)
        {
            item.Anim.SetTrigger("Exit");
        }

        public void Execute(WorldItem item)
        {
            item.transform.position = PlayerManager.PlayerPos;
            _timer += Time.deltaTime;
            if (_timer < _releaseTime) return;
            
            _poolManager.ReleasePoolObject(item);
        }

        public void FixedExecute(WorldItem item)
        {
            
        }
    }
}