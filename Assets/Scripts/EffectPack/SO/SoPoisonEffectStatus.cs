using EnchantmentPack.Enums;
using Other;
using PlayerPack;
using UnityEngine;

namespace EffectPack.SO
{
    [CreateAssetMenu(fileName = "new Poison Effect", menuName = "Custom/EffectStatus/Poison")]
    public class SoPoisonEffectStatus : SoEffectBase
    {
        [SerializeField] private int poisonDamage;
        
        public override void OnResolve(EffectsManager effectsManager, int stacks, CanBeDamaged canBeDamaged)
        {
            var canStack = PlayerManager.PlayerEnchantments.Has(EEnchantmentName.PoisonCanStack);
            stacks = canStack ? stacks : 1;
            canBeDamaged.GetDamaged(poisonDamage * stacks);
        }
    }
}