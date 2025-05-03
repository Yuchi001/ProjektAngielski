using UnityEngine;

namespace EnemyPack.States.StateData
{
    public class PatrolStateData : StateDataBase
    {
        [SerializeField] private float patrolMovementSpeed;
        [SerializeField] private float patrolRange;

        public float PatrolMovementSpeed => patrolMovementSpeed;
        public float PatrolRange => patrolRange;
    }
}