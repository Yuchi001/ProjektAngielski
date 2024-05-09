using EnemyPack;
using UnityEngine;
using WeaponPack.Enums;

namespace WeaponPack.WeaponsLogic
{
    public class BookOfDestructionLogic : WeaponLogicBase
    {
        private float Range => GetStatValue(EWeaponStat.BlastRange) ?? 0;
        private float DropExpChance => GetStatValue(EWeaponStat.DropExpChance) ?? 0;

        protected override void UseWeapon()
        {
            var foundObjects = Physics2D.OverlapCircleAll(PlayerPos, Range);
            foreach (var colliderObj in foundObjects)
            {
                var randomNum = Random.Range(0, 101);
                var success = colliderObj.TryGetComponent<EnemyLogic>(out var enemy);
                if(!success) continue;
                if(randomNum < DropExpChance) enemy.OnDie();
                else enemy.DieWithoutGem();
            }
        }
    }
}