using System;
using PlayerPack;
using UnityEngine;

namespace InventoryPack.WorldItemPack
{
    public class PickUpState : IWorldItemState
    {
        private readonly float _pickUpTime;
        private readonly Func<LiveState> _lastState;
        private readonly Func<ReleaseState> _nextState;
        
        private Vector2 _startPos;
        private float _timer = 0;

        public PickUpState(float pickUpTime, Func<LiveState> lastState, Func<ReleaseState> nextState)
        {
            _pickUpTime = pickUpTime;
            _lastState = lastState;
            _nextState = nextState;
        }
        
        public void Enter(WorldItem item)
        {
            _timer = 0;
            _startPos = item.transform.position;
            item.Anim.SetTrigger("PickUp");
        }

        public void Execute(WorldItem item)
        {
            _timer += Time.deltaTime;
            var currentTime = Mathf.Clamp01(_timer / _pickUpTime);
            item.transform.position = Vector2.Lerp(_startPos, PlayerManager.PlayerPos, currentTime);
            if (currentTime < 1f) return;
            
            var pickedUp = item.TryPickUp();
            if (!pickedUp) item.SwitchState(_lastState.Invoke());
            else item.SwitchState(_nextState.Invoke());
        }

        public void FixedExecute(WorldItem item)
        {
            
        }
    }
}