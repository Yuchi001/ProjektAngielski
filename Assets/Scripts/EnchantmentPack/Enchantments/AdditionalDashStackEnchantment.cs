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
            var playerInstance = PlayerManager.Instance;
            yield return new WaitUntil(() => playerInstance != null);
            playerInstance.PlayerMovement.AddDashStack();
        }
    }
}