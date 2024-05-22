using System;
using EnemyPack.SO;
using UnityEngine;

namespace EnemyPack
{
    public abstract class EnemyAction : MonoBehaviour
    {
        private float _timer = 0;

        protected EnemyLogic EnemyLogic => _enemyLogic;
        private EnemyLogic _enemyLogic;

        protected SoEnemy EnemyValues => _enemyValues;
        private SoEnemy _enemyValues;
        
        protected const int EnemyProjectileLayerMask = 7;
        protected const string PlayerTagName = "Player";
        
        public virtual void Setup(EnemyLogic enemyLogic, SoEnemy enemyValues)
        {
            _enemyValues = enemyValues;
            _enemyLogic = enemyLogic;
        }

        private void Update()
        {
            OnUpdate();
            
            _timer += Time.deltaTime;
            if (_timer < EnemyValues.ActionCooldown) return;

            _timer = 0;
            InvokeAction();
        }

        protected abstract void InvokeAction();

        protected virtual void OnUpdate()
        {
        }
    }
}