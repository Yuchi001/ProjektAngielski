using ItemPack.Enums;
using ItemPack.WeaponPack.Other;
using Other.Enums;
using UnityEngine;
using WeaponPack.Enums;

namespace ItemPack.WeaponPack.WeaponsLogic
{
    public class WhipLogic : ItemLogicBase
    {
        [SerializeField] private bool useEffect;
        [SerializeField] private EEffectType effectType;
        [SerializeField] private GameObject slashPrefab;
        
        private float EffectDuration => GetStatValue(EWeaponStat.EffectDuration) ?? 0;
        private float ProjectileScale => GetStatValue(EWeaponStat.ProjectileScale) ?? 0;
        
        protected override bool Use()
        {
            var slashObject = Instantiate(slashPrefab, PlayerPos, Quaternion.identity);
            var slashScript = slashObject.GetComponent<Slash>();

            slashScript.Setup(Damage, ProjectileScale)
                .SetPushForce(PushForce)
                .SetEffect(useEffect ? effectType : null, EffectDuration)
                .Ready();
                
            return true;
        }
    }
}