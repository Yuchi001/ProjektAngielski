using UnityEngine;

namespace EnemyPack.States.StateData
{
    public class AttackStateData : StateDataBase
    {
        [SerializeField] protected float range;
        [SerializeField] protected float attackRate;
        [SerializeField] protected int damage;

        public float Range => range;
        public float AttackRate => attackRate;
        public int Damage => damage;
    }
}