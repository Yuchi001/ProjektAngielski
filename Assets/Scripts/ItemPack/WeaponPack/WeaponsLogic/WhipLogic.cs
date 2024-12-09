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

        private static Slash SlashPrefab => GameManager.Instance.GetPrefab<Slash>(PrefabNames.SlashAttack);
        
        private float EffectDuration => GetStatValue(EItemSelfStatType.EffectDuration) ?? 0;
        private float ProjectileScale => GetStatValue(EItemSelfStatType.ProjectileScale) ?? 0;
        
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