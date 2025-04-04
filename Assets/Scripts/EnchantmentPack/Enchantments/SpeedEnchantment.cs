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
            yield return new WaitUntil(PlayerManager.HasInstance);
            
            var percentage =
                PlayerManager.PlayerEnchantments.GetParamValue(Enchantment.EnchantmentName, EValueKey.Percentage);
            PlayerManager.PlayerMovement.ModifySpeedByPercentage(percentage);
        }
    }
}