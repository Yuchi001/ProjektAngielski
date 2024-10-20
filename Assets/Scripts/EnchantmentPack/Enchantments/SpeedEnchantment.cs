using System.Collections;
using EnchantmentPack.Enums;
using PlayerPack;
using UnityEngine;

namespace EnchantmentPack.Enchantments
{
    public class SpeedEnchantment : EnchantmentBase
    {
        private IEnumerator Start()
        {
            var playerManager = PlayerManager.Instance;
            yield return new WaitUntil(() => playerManager != null);

            var enchantmentManager = playerManager.PlayerEnchantments;
            var percentage =
                enchantmentManager.GetParamValue(Enchantment.EnchantmentName, EValueKey.Percentage);
            playerManager.PlayerMovement.ModifySpeedByPercentage(percentage);
        }
    }
}