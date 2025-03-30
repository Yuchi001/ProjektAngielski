using System.Collections.Generic;
using System.Linq;
using ItemPack.Enums;
using ItemPack.WeaponPack.Other;
using Managers;
using Managers.Other;
using Other.Enums;
using UnityEngine;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class WhipLogic : ItemLogicBase
    {
        [SerializeField] private bool useEffect;
        [SerializeField] private EEffectType effectType;

        private static Slash _slashPrefab;

        private static Slash SlashPrefab
        {
            get
            {
                if (_slashPrefab == null) _slashPrefab = GameManager.Instance.GetPrefab<Slash>(PrefabNames.SlashAttack);
                return _slashPrefab;
            }
        }

        private float EffectDuration => GetStatValue(EItemSelfStatType.EffectDuration);
        private float ProjectileScale => GetStatValue(EItemSelfStatType.ProjectileScale);
        
        protected override List<EItemSelfStatType> UsedStats { get; } = new()
        {
            EItemSelfStatType.EffectDuration,
            EItemSelfStatType.ProjectileScale,
            EItemSelfStatType.PushForce,
            EItemSelfStatType.ProjectilesCount
        };

        protected override bool Use()
        {
            var slash = Instantiate(SlashPrefab, PlayerPos, Quaternion.identity);

            slash.Setup(Damage, ProjectileScale)
                .SetPushForce(PushForce)
                .SetEffect(useEffect ? effectType : null, EffectDuration)
                .Ready();
                
            return true;
        }
    }
}