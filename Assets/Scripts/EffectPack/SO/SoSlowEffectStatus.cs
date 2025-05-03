using Other;
using UnityEngine;

namespace EffectPack.SO
{
    [CreateAssetMenu(fileName = "new Slow Effect", menuName = "Custom/EffectStatus/Slow")]
    public class SoSlowEffectStatus : SoEffectBase
    {
        public override void OnResolve(EffectsManager effectsManager, int stacks, CanBeDamaged canBeDamaged, int additionalDamage)
        {
            if (additionalDamage != 0) canBeDamaged.GetDamaged(additionalDamage);
        }
    }
}