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
        
        public static void BuyItem(int offerIndex)
        {
            PlayerCollectibleManager.ModifyCollectibleAmount(PlayerCollectibleManager.ECollectibleType.COIN, -Instance._offers[offerIndex].Price);
            Instance._offers[offerIndex] = null;
        }

        public static bool CanAfford(int offerIndex)
        {
            var offer = Instance._offers[offerIndex];
            if (offer == null) return false;
            return PlayerCollectibleManager.GetCollectibleCount(PlayerCollectibleManager.ECollectibleType.COIN) >= offer.Price;
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
        }
    }
}