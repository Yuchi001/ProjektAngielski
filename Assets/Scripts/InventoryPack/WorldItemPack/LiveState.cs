using System;
using ItemPack.Enums;
using PlayerPack;
using PoolPack;
using UnityEngine;
using Utils;

namespace InventoryPack.WorldItemPack
{
    public class LiveState : IWorldItemState
    {
        private readonly float _pickUpDistance;
        private readonly float _baseLifeTime;
        private readonly PoolManager _poolManager;
        private readonly Func<PickUpState> _nextState;
        
        private float _lifeTime;
        private float _lifeTimeTimer = 0;
        
        public LiveState(float pickUpDistance, float baseLifeTime, PoolManager poolManager, Func<PickUpState> nextState)
        {
            _pickUpDistance = pickUpDistance;
            _baseLifeTime = baseLifeTime;
            _poolManager = poolManager;
            _nextState = nextState;
        }
        
        public void Enter(WorldItem item)
        {
            _lifeTimeTimer = 0;
            _lifeTime = item.Item.WorldLifeTime > 0 ? item.Item.WorldLifeTime : _baseLifeTime;
        }

        public void Execute(WorldItem item)
        {
            _lifeTimeTimer += Time.deltaTime;
            if (_lifeTimeTimer < _lifeTime) return;
            
            _poolManager.ReleasePoolObject(item);
        }

        public void LazyExecute(WorldItem item, float lazyDeltaTime)
        {
            if (!item.Item.CanPickUp() || item.transform.Distance(PlayerManager.PlayerPos) > _pickUpDistance) return;
            item.SwitchState(_nextState.Invoke());
        }
    }
}