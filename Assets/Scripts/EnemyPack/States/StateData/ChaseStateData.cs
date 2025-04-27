using UnityEngine;

namespace EnemyPack.States.StateData
{
    public class ChaseStateData : StateDataBase
    {
        [SerializeField] private float chaseMovementSpeed;
        [SerializeField] private float chaseDetectionRange;
        [SerializeField] private float chaseStopRange;

        public float ChaseMovementSpeed => chaseMovementSpeed;
        public float ChaseDetectionRange => chaseDetectionRange;
        public float ChaseStopRange => chaseStopRange;
    }
}