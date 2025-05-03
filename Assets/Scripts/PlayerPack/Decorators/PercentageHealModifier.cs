using PlayerPack.Interface;
using UnityEngine;

namespace PlayerPack.Decorators
{
    public class PercentageHealModifier : IHealModifier
    {
        bool IHealModifier.QueueAsLast { get; set; } = true;
        private readonly float value;

        public PercentageHealModifier(float value)
        {
            this.value = value;
        }

        public void ModifyHealContext(HealContext healContext)
        {
            healContext.ModifyValue(Mathf.CeilToInt(value * healContext.Value));
        }
    }
}