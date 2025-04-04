using System;
using System.Collections;
using PlayerPack;
using UnityEngine;

namespace EnchantmentPack.Enchantments
{
    public class AdditionalDashStackEnchantment : EnchantmentBase
    {
        private IEnumerator Start()
        {
            yield return new WaitUntil(PlayerManager.HasInstance);
            PlayerManager.PlayerMovement.AddDashStack();
        }
    }
}