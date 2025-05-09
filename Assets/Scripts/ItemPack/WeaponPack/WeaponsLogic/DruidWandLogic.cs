﻿using System.Collections.Generic;
using System.Linq;
using EnemyPack;
using ItemPack.Enums;
using PlayerPack;
using UnityEngine;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class DruidWandLogic : ItemLogicBase
    {
        private float Range => GetStatValue(EItemSelfStatType.BlastRange);
        private float HealValue => GetStatValue(EItemSelfStatType.HealValue);

        protected override List<EItemSelfStatType> UsedStats { get; } = new()
        {
            EItemSelfStatType.BlastRange,
            EItemSelfStatType.HealValue,
            EItemSelfStatType.PushForce
        };

        public override IEnumerable<EItemSelfStatType> GetUsedStats()
        {
            var stats = base.GetUsedStats().ToList();
            stats.Remove(EItemSelfStatType.Damage);
            return stats.Concat(UsedStats);
        }

        protected override bool Use()
        {
            var results = new Collider2D[20];
            Physics2D.OverlapCircleNonAlloc(PlayerPos, Range, results);
            foreach (var hit in results)
            {
                if(hit == null || !hit.TryGetComponent(out EnemyLogic enemyLogic)) continue;


                enemyLogic.PushEnemy(PlayerPos, PushForce);
            }
            
            PlayerManager.PlayerHealth.Heal((int)HealValue);

            return true;
        }
    }
}