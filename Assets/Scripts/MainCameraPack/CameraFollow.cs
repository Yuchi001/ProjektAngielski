using System;
using PlayerPack;
using UnityEngine;

namespace MainCameraPack
{
    public class CameraFollow : MonoBehaviour
    {
        private void Update()
        {
            if (PlayerManager.Instance == null || PlayerManager.Instance.PlayerHealth.Dead) return;

            transform.position = PlayerManager.Instance.transform.position;
        }
    }
}