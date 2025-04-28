using System;
using System.Collections.Generic;
using System.Linq;
using ItemPack.Enums;
using ItemPack.SO;
using Managers;
using PlayerPack;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using Random = UnityEngine.Random;

namespace ShopPack
{
    public class ShopManager : MonoBehaviour
    {
        [SerializeField] private int offersCount;
        [SerializeField] private float stagePriceMultiplier;
        [SerializeField] private MinMax baseWeaponPrice;
        [SerializeField] private MinMax baseScrapPrice;
        [SerializeField] private int refreshCost;

        [SerializeField] private SoScrapItem scrapItem;

        private List<ShopOffer> _offers;
        
        private static int StageCount => GameManager.StageCount;
        public static int OffersCount => Instance.offersCount;
        public static int RefreshCost => Instance.refreshCost;

        private static ShopManager Instance { get; set; }
        
        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        public static List<ShopOffer> GetOffers()
        {
            return Instance._offers;
        }

        public static void RefreshOffers(bool includeValidOffers = false)
        {
            if (Instance._offers == null)
            {
                Instance._offers = new List<ShopOffer>();
                for (var i = 0; i < Instance.offersCount; i++)
                {
                    Instance._offers.Add(Instance.CreateOffer());
                }

                return;
            }

            for (var i = 0; i < Instance._offers.Count; i++)
            {
                if (Instance._offers[i] != null && !includeValidOffers) continue;

                Instance._offers[i] = Instance.CreateOffer();
            }
        }

        private ShopOffer CreateOffer()
        {
            var isScrap = Random.Range(0, 2) == 1;
            SoItem item = isScrap ? scrapItem : PlayerManager.PlayerItemManager.GetRandomItems(1).ElementAt(0);
            var param = Random.Range(1, StageCount + 1);
            var price = GetPrice(isScrap, param);
            return new ShopOffer(item, price, param);
        }

        private int GetPrice(bool isScrap, int param)
        {
            var price = 0;
            for (var i = 0; i < param; i++)
            {
                price += isScrap ? baseScrapPrice.RandomInt() : baseWeaponPrice.RandomInt();
            }

            return Mathf.CeilToInt(price * stagePriceMultiplier * StageCount);
        }

        public static bool BuyItem(int index)
        {
            var success = Instance._offers[index].Buy();
            if (!success) return false;
            
            PlayerCollectibleManager.ModifyCollectibleAmount(PlayerCollectibleManager.ECollectibleType.COIN, -Instance._offers[index].Price);
            Instance._offers[index] = null;
            return true;
        }

        public class ShopOffer
        {
            public readonly SoItem Item;
            public readonly int Price;
            public readonly int Param; // level if invItem, count if worldItem
            
            public ShopOffer(SoItem item, int price, int param)
            {
                Item = item;
                Price = price;
                Param = param;
            }

            public bool Buy()
            {
                if (Item.ItemType != EItemType.WorldOnlyItem && PlayerManager.PlayerItemManager.IsFull()) return false;

                var coinCount = PlayerCollectibleManager.GetCollectibleCount(PlayerCollectibleManager.ECollectibleType.COIN); 
                if (coinCount < Price) return false;
                
                Item.OnPickUp(Param);
                return true;
            }
        }
    }
}