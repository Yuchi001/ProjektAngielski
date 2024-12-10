using System.Collections;
using System.Collections.Generic;
using AudioPack;
using EnchantmentPack.Enums;
using EnemyPack.CustomEnemyLogic;
using ItemPack.Enums;
using ItemPack.WeaponPack.SideClasses;
using Managers;
using Managers.Enums;
using SpecialEffectPack;
using SpecialEffectPack.Enums;
using UnityEngine;
using WeaponPack.Enums;
using Random = UnityEngine.Random;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class BookOfDestructionLogic : ItemLogicBase
    {
        [SerializeField] private GameObject destructionCirclePrefab;
        private float Range => GetStatValue(EItemSelfStatType.BlastRange) ?? 0;
        private float DropExpChance => GetStatValue(EItemSelfStatType.DropExpChance) ?? 0;

        private void Start()
        {
            var prefab = Instantiate(destructionCirclePrefab);
            prefab.GetComponent<DestructionCircle>().Setup(this);
        }

        protected override bool Use()
        {
            var results = new Collider2D[100];
            var size = Physics2D.OverlapCircleNonAlloc(PlayerPos, Range, results);

            StartCoroutine(QueueDeaths(results));

            return size > 0;
        }

        private IEnumerator QueueDeaths(IEnumerable<Collider2D> targets)
        {
            foreach (var colliderObj in targets)
            {
                if(colliderObj == null) continue;

                AudioManager.Instance.PlaySound(ESoundType.BulletExplode);
                SpecialEffectManager.Instance.SpawnExplosion(ESpecialEffectType.ExplosionMedium,
                    colliderObj.transform.position, 0.3f);
                
                var randomNum = Random.Range(0, 101);
                var success = colliderObj.TryGetComponent<EnemyLogic>(out var enemy);
                if(!success) continue;
                
                enemy.SpawnBlood();
                if(randomNum < DropExpChance) enemy.OnDie();
                else enemy.DieWithoutGem();

                yield return new WaitForSeconds(0.05f);
            }
        } 

        public float GetRange()
        {
            return Range;
        }
        
        protected override float CustomCooldownModifier()
        {
            var stacks = PlayerEnchantments.GetStacks(EEnchantmentName.BetterBooks);
            if (stacks <= 0) return base.CustomCooldownModifier();
            var percentage = PlayerEnchantments.GetParamValue(EEnchantmentName.BetterBooks, EValueKey.Percentage);
            return 1 + percentage * stacks;
        }
    }
}