using Other;
using UnityEngine;

namespace EffectPack.SO
{
    [CreateAssetMenu(fileName = "new Stun Effect", menuName = "Custom/EffectStatus/Stun")]
    public class SoStunEffectStatus : SoEffectBase
    {
        public override void OnResolve(EffectsManager effectsManager, int stacks, CanBeDamaged canBeDamaged, int additionalDamage)
        {
            if (additionalDamage != 0) canBeDamaged.GetDamaged(additionalDamage);
        }
    }
}