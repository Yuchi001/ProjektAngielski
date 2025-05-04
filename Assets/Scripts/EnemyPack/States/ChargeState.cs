using System;
using System.Collections.Generic;
using EnemyPack.SO;
using EnemyPack.States.StateData;
using PlayerPack;
using UnityEngine;
using Utils;
using WorldGenerationPack;

namespace EnemyPack.States
{
    public class ChargeState : StateBase
    {
        private readonly ChargeStateData _data;
        private readonly Func<StateBase> _nextState;

        private Vector2 _direction;
        private float _timer = 0;
        private bool _didDamagePlayer = false;
        
        public ChargeState(SoEnemy data, Func<StateBase> nextState) : base(data)
        {
            _nextState = nextState;
            _data = data.GetStateData<ChargeStateData>();
        }

        public override void Enter(EnemyLogic state, StateBase lastState)
        {
            _direction = (PlayerPos - (Vector2)state.transform.position).normalized;
            _didDamagePlayer = false;
            _timer = 0;
            
            state.Animator.enabled = false;
            if (_data.UseFreezeSprite) state.SpriteRenderer.sprite = _data.FreezeSprite;
        }

        public override void Execute(EnemyLogic state)
        {
            var toPlayerNow = (PlayerPos - (Vector2)state.transform.position).normalized;
            var dot = Vector2.Dot(_direction, toPlayerNow);

            if (dot < 0f)
            {
                state.SwitchState(_nextState.Invoke());
                state.Animator.enabled = true;
                state.Animator.Play("Idle");
                state.SetInvincible(false);
                state.SwitchState(_nextState.Invoke());
                return;
            }
            
            _timer += state.deltaTime;
            if (_timer > 1f / _data.ChargePositionUpdateRate)
            {
                _timer = 0;
                var currentToPlayer = (PlayerPos - (Vector2)state.transform.position).normalized;
                _direction = Vector3.Lerp(_direction, currentToPlayer, _data.ChargePrecision);
            }

            var separation = Vector2.zero;
            var minSeparationDistance = 1.5f;

            var nearbyEnemies = new List<EnemyLogic>();
            EnemyManager.GetNearbyEnemies(state.transform.position, minSeparationDistance, ref nearbyEnemies);
            foreach (var poolObj in nearbyEnemies)
            {
                if (poolObj.gameObject == state.transform.gameObject) continue;

                var pushAway = (Vector2)(state.transform.position - poolObj.transform.position);
                var dist = pushAway.magnitude;

                if (dist < minSeparationDistance)
                {
                    var bodySizeFactor = state.EnemyData.BodyScale * state.transform.localScale.x;
                    separation += pushAway.normalized * ((minSeparationDistance - dist) * bodySizeFactor);
                }
                else
                {
                    separation += pushAway.normalized * 0.05f;
                }
            }
            
            var finalDir = (_direction + separation * (0.1f * state.transform.localScale.x)).normalized;
            state.transform.position += (Vector3)(finalDir * (_data.ChargeMovementSpeed * state.deltaTime));
            
            if (!_didDamagePlayer && InRange(state, _data.ChargeAttackRange))
            {
                _didDamagePlayer = true;
                PlayerManager.PlayerHealth.GetDamaged(_data.ChargeDamage);
            }
        }

        public override void Reset(EnemyLogic state)
        {
            _didDamagePlayer = false;
            _timer = 0;
        }
    }
}