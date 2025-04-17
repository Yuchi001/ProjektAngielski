using System.Collections.Generic;
using ItemPack.Enums;
using ItemPack.WeaponPack.Other;
using Managers;
using Managers.Other;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class BookOfFireLogic : ItemLogicBase
    {
        private FireField _spawnedFireField = null;

        public float Range => GetStatValue(EItemSelfStatType.ProjectileRange);
        public float DamageRate => GetStatValue(EItemSelfStatType.DamageRate);
        public float EffectDuration => GetStatValue(EItemSelfStatType.EffectDuration);
        
        protected override List<EItemSelfStatType> UsedStats { get; } = new()
        {
            EItemSelfStatType.ProjectileRange,
            EItemSelfStatType.DamageRate,
            EItemSelfStatType.EffectDuration,
            EItemSelfStatType.Damage
        };

        protected override bool Use()
        {
            if (_spawnedFireField != null) return false;

            var prefab = GameManager.GetPrefab<FireField>(PrefabNames.FireField);
            _spawnedFireField = Instantiate(prefab);
            _spawnedFireField.Setup(this);

            return false;
        }

        public override void Remove()
        {
            Destroy(_spawnedFireField.gameObject);
            base.Remove();
        }
    }
}