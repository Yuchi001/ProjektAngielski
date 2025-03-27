using System;
using Managers;
using Managers.Other;
using UnityEngine;

namespace PlayerPack
{
    public class PlayerDashAnim : MonoBehaviour
    {
        [SerializeField] private float spriteLifeTime;
        [SerializeField] private float spriteSpawnRate;
        [SerializeField] private Transform spriteTransform;

        private PlayerDashSprite _playerDashSpritePrefab;
        private bool _spawn = false;
        private float _timer = 0;

        private void Start()
        {
            _playerDashSpritePrefab = GameManager.Instance.GetPrefab<PlayerDashSprite>(PrefabNames.DashAnimSprite);
        }

        public void StartAnim()
        {
            _spawn = true;
            _timer = 0;
            SpawnFrame();
        }

        public void StopAnim()
        {
            _spawn = false;
            SpawnFrame();
        }

        private void Update()
        {
            if (!_spawn) return;

            _timer += Time.deltaTime;
            if (_timer < 1f / spriteSpawnRate) return;
            _timer = 0;
            
            SpawnFrame();
        }

        private void SpawnFrame()
        {
            Instantiate(_playerDashSpritePrefab, spriteTransform.position, Quaternion.identity).Setup(spriteLifeTime);
        }
    }
}