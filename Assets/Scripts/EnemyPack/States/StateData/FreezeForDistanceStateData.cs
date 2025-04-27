using UnityEngine;

namespace EnemyPack.States.StateData
{
    public class FreezeForDistanceStateData : FreezeStateData
    {
        [SerializeField] private float freezeDetectionRange;
        [SerializeField] private bool isInvincibleDuringFreeze;

        public float FreezeDetectionRange => freezeDetectionRange;
        public bool IsInvincibleDuringFreeze => isInvincibleDuringFreeze;
    }
}