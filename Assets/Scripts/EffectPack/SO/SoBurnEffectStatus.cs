using Other;
using UnityEngine;

namespace EffectPack.SO
{
    [CreateAssetMenu(fileName = "new Burn Effect", menuName = "Custom/EffectStatus/Burn")]
    public class SoBurnEffectStatus : SoEffectBase
    {
        [SerializeField] private int burnDamage;
        public override void OnResolve(EffectsManager effectsManager, int stacks, CanBeDamaged canBeDamaged)
        {
            canBeDamaged.GetDamaged(burnDamage);
        }
    }
}