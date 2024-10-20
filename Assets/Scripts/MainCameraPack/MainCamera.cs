using System;
using PlayerPack;
using UnityEngine;

namespace MainCameraPack
{
    public class MainCamera : MonoBehaviour
    {
        [SerializeField] private CameraShaker _cameraShaker;
        private Transform _player;

        private void Awake()
        {
            PlayerHealth.OnPlayerDamaged += ShakeCamera;
        }

        private void OnDisable()
        {
            PlayerHealth.OnPlayerDamaged -= ShakeCamera;
        }

        public void Setup(GameObject player)
        {
            _player = player.transform;
        }

        private void ShakeCamera()
        {
            _cameraShaker.ShakeOnce(10, 10, 0.1f, 0.1f);
        }

        private void Update()
        {
            if (_player == null) return;

            transform.position = new Vector3(_player.position.x, _player.position.y, -10);
        }
    }
}