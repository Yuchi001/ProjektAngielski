using UnityEngine;

namespace EnemyPack.States.StateData
{
    public class MeleeAttackStateData : StateDataBase
    {
        [SerializeField] protected float attackRange;
        [SerializeField] protected float attackRate;
        [SerializeField] protected int damage;

        public float AttackRange => attackRange;
        public float AttackRate => attackRate;
        public int Damage => damage;
    }
}