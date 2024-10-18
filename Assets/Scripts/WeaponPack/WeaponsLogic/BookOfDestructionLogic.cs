using System.Collections;
using System.Collections.Generic;
using EnchantmentPack.Enums;
using EnemyPack.CustomEnemyLogic;
using Managers;
using Managers.Enums;
using SpecialEffectPack;
using SpecialEffectPack.Enums;
using UnityEngine;
using WeaponPack.Enums;
using WeaponPack.SideClasses;
using Random = UnityEngine.Random;

namespace WeaponPack.WeaponsLogic
{
    public class BookOfDestructionLogic : WeaponLogicBase
    {
        [SerializeField] private GameObject destructionCirclePrefab;
        private float Range => GetStatValue(EWeaponStat.BlastRange) ?? 0;
        private float DropExpChance => GetStatValue(EWeaponStat.DropExpChance) ?? 0;

        private void Start()
        {
            var prefab = Instantiate(destructionCirclePrefab);
            prefab.GetComponent<DestructionCircle>().Setup(this);
        }

        protected override bool UseWeapon()
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
            var stacks = PlayerEnchantmentManager.GetStacks(EEnchantmentName.BetterBooks);
            if (stacks <= 0) return base.CustomCooldownModifier();
            return 1 + GameManager.Instance.EnchantmentValueDictionary[EEnchantmentName.BetterBooks][EValueKey.Percentage] * stacks;
        }
    }
}