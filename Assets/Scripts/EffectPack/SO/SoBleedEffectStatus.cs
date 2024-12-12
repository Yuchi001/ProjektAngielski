using Other;
using UnityEngine;

namespace EffectPack.SO
{
    [CreateAssetMenu(fileName = "new Bleed Effect", menuName = "Custom/EffectStatus/Bleed")]
    public class SoBleedEffectStatus : SoEffectBase
    {
        public override void OnResolve(EffectsManager effectsManager, int stacks, CanBeDamaged canBeDamaged)
        {
            canBeDamaged.GetDamaged(1 * stacks);
        }
    }
}