using System;
using System.Collections.Generic;
using DamageIndicatorPack;
using InventoryPack;
using Managers;
using Managers.Other;
using PlayerPack;
using StructurePack;
using StructurePack.SO;
using UIPack.Elements;
using UnityEngine;
using UnityEngine.UI;

namespace ShopPack
{
    public class ShopUI : Box, IStructure
    {
        [SerializeField] private RectTransform offerHolder;
        [SerializeField] private TextButton refreshButton;

        private readonly List<OfferWindow> _spawnedOfferWindows = new();
        
        protected override void Awake()
        {
            _spawnedOfferWindows.Clear();
            var offerWindowPrefab = GameManager.GetPrefab<OfferWindow>(PrefabNames.OfferWindow);
            for (var i = 0; i < ShopManager.OffersCount; i++)
            {
                var offerWindow = Instantiate(offerWindowPrefab, offerHolder);
                _spawnedOfferWindows.Add(offerWindow);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(offerHolder);
            
            InitBox();
        }
        
        public void Setup(SoStructure structureData, StructureBase structureBase)
        {
            
        }

        protected override Vector2 GetItemDropPosition()
        {
            return PlayerManager.PlayerPos;
        }
        
        public override void OnOpen(string key)
        {
            base.OnOpen(key);
            Time.timeScale = 0;

            refreshButton.SetText($"R E F R E S H {ShopManager.RefreshCost}$");
            PopulateOfferWindows();
        }

        public override void OnClose()
        {
            base.OnClose();
            Time.timeScale = 1;
        }

        private void PopulateOfferWindows()
        {
            var offers = ShopManager.GetOffers();
            for (var i = 0; i < ShopManager.OffersCount; i++)
            {
                var offer = offers[i];
                _spawnedOfferWindows[i].Setup(offer, i);
            }
        }

        public void RefreshOffers()
        {
            var coinCount = PlayerCollectibleManager.GetCollectibleCount(PlayerCollectibleManager.ECollectibleType.COIN);
            if (coinCount < ShopManager.RefreshCost)
            {
                IndicatorManager.SpawnIndicator(refreshButton.transform.position, "Not enough coins!", Color.red);
                return;
            }
            
            PlayerCollectibleManager.ModifyCollectibleAmount(PlayerCollectibleManager.ECollectibleType.COIN, -ShopManager.RefreshCost);
            ShopManager.RefreshOffers(true);
            PopulateOfferWindows();
        }
    }
}