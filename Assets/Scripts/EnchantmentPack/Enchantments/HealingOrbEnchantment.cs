using EnchantmentPack.Enums;
using EnemyPack;
using InventoryPack.WorldItemPack;
using UnityEngine;

namespace EnchantmentPack.Enchantments
{
    public class HealingOrbEnchantment : EnchantmentBase
    {
        [SerializeField] private float gemPosOffset = 0.3f;
        [SerializeField] private GameObject healingOrbPrefab;
        private void Awake()
        {
            EnemySpawner.OnEnemyDie += TriggerPoisonSpread;
        }

        private void OnDisable()
        {
            EnemySpawner.OnEnemyDie -= TriggerPoisonSpread;
        }

        private void TriggerPoisonSpread(EnemyLogic enemyLogic)
        {
            if (Random.Range(0f, 1f) > parameters[EValueKey.Percentage]) return;

            var position = new Vector3
            {
                x = Random.Range(-gemPosOffset, gemPosOffset),
                y = Random.Range(-gemPosOffset, gemPosOffset)
            };
            
            WorldItemManager.SpawnHealingOrb(position);
        }
    }
}