﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AudioPack;
using EnemyPack;
using ItemPack.Enums;
using ItemPack.WeaponPack.SideClasses;
using Managers.Enums;
using ProjectilePack;
using SpecialEffectPack;
using SpecialEffectPack.Enums;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class BookOfDestructionLogic : ItemLogicBase
    {
        [SerializeField] private GameObject destructionCirclePrefab;
        private float Range => GetStatValue(EItemSelfStatType.BlastRange);
        private float DropExpChance => GetStatValue(EItemSelfStatType.DropExpChance);

        private void Start()
        {
            var prefab = Instantiate(destructionCirclePrefab);
            prefab.GetComponent<DestructionCircle>().Setup(this);
        }

        protected override List<EItemSelfStatType> UsedStats { get; } = new()
        {
            EItemSelfStatType.BlastRange,
            EItemSelfStatType.DropExpChance
        };
        
        public override IEnumerable<EItemSelfStatType> GetUsedStats()
        {
            return base.GetUsedStats().Concat(_otherDefaultStatsNoPush);
        }

        protected override bool Use()
        {
            var results = TargetDetector.EnemiesInRange(PlayerPos, Range);
            StartCoroutine(QueueDeaths(results));

            return results.Count > 0;
        }

        private IEnumerator QueueDeaths(IEnumerable<EnemyLogic> targets)
        {
            foreach (var enemy in targets)
            {
                if(enemy == null) continue;

                AudioManager.PlaySound(ESoundType.BulletExplode);
                SpecialEffectManager.SpawnExplosion(ESpecialEffectType.ExplosionMedium,
                    enemy.transform.position, 0.3f);
                
                var randomNum = Random.Range(0, 101);
                
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
    }
}