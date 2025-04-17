using Other;
using UnityEngine;

namespace EffectPack.SO
{
    [CreateAssetMenu(fileName = "new Poison Effect", menuName = "Custom/EffectStatus/Poison")]
    public class SoPoisonEffectStatus : SoEffectBase
    {
        [SerializeField] private int poisonDamage;
        
        public override void OnResolve(EffectsManager effectsManager, int stacks, CanBeDamaged canBeDamaged)
        {
            canBeDamaged.GetDamaged(poisonDamage * 1);
        }
    }
}