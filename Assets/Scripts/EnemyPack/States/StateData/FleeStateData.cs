using UnityEngine;

namespace EnemyPack.States.StateData
{
    public sealed class FleeStateData : StateDataBase
    {
        [SerializeField] private float fleeMovementSpeed;
        [SerializeField] private float fleeDetectionRange;

        public float FleeMovementSpeed => fleeMovementSpeed;
        public float FleeDetectionRange => fleeDetectionRange;
    }
}