using Other;
using UnityEngine;

namespace EffectPack.SO
{
    [CreateAssetMenu(fileName = "new Bleed Effect", menuName = "Custom/EffectStatus/Bleed")]
    public class SoBleedEffectStatus : SoEffectBase
    {
        public override void OnResolve(EffectsManager effectsManager, int stacks, CanBeDamaged canBeDamaged, int additionalDamage)
        {
            canBeDamaged.GetDamaged((1 + additionalDamage) * stacks);
        }
    }
}