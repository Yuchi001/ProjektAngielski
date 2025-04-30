using UnityEngine;

namespace EnemyPack.States.StateData
{
    public class FreezeForDistanceStateData : FreezeStateData
    {
        [SerializeField] private float freezeDetectionRange;

        public float FreezeDetectionRange => freezeDetectionRange;
    }
}