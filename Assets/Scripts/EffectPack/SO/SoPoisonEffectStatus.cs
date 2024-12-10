using Other;
using UnityEngine;

namespace EffectPack.SO
{
    [CreateAssetMenu(fileName = "new Poison Effect", menuName = "Custom/EffectStatus/Poison")]
    public class SoPoisonEffectStatus : SoEffectBase
    {
        public override void OnResolve(EffectsManager effectsManager, int stacks, CanBeDamaged canBeDamaged)
        {
            
        }
    }
}