using Managers.Enums;
using PlayerPack;
using PlayerPack.Enums;
using UnityEngine;

namespace ItemPack.SO
{
    [CreateAssetMenu(fileName = "new HealingOrbItem", menuName = "Custom/Item/HealingOrbItem")]
    public class SoHealingOrbItem : SoItem
    {
        [SerializeField] private int healValue;
        public override bool OnPickUp(params int[] paramArray)
        {
            PlayerManager.PlayerHealth.Heal(healValue, ESoundType.HealOrb);
            return true;
        }

        public override bool CanPickUp()
        {
            var sm = PlayerManager.PlayerStatsManager;
            return sm.GetStatAsInt(EPlayerStatType.Health) + healValue <= sm.GetStatAsInt(EPlayerStatType.MaxHealth);
        }
    }
}