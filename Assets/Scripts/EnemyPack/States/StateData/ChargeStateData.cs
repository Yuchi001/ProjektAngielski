using UnityEngine;

namespace EnemyPack.States.StateData
{
    public class ChargeStateData : StateDataBase
    {
        [SerializeField] private float chargeAttackRange;
        [SerializeField] private float chargeMovementSpeed;
        [SerializeField] private float chargePositionUpdateRate;
        [SerializeField, Range(0f, 1f)] private float chargePrecision;
        [SerializeField] private int chargeDamage;
        [SerializeField] private bool useFreezeSprite = true;
        [SerializeField] private Sprite freezeSprite;

        public float ChargeAttackRange => chargeAttackRange;
        public float ChargeMovementSpeed => chargeMovementSpeed;
        public float ChargePositionUpdateRate => chargePositionUpdateRate;
        public int ChargeDamage => chargeDamage;
        public float ChargePrecision => chargePrecision;
        public bool UseFreezeSprite => useFreezeSprite;
        public Sprite FreezeSprite => freezeSprite;
    }
}