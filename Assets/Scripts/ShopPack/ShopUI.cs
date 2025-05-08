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
        [SerializeField] private RefreshOffersButton refreshButton;

        private readonly List<OfferWindow> _spawnedOfferWindows = new();
        private readonly List<IShopUIElement> _uiElements = new();
        
        protected override void Awake()
        {
            _spawnedOfferWindows.Clear();
            var offerWindowPrefab = GameManager.GetPrefab<OfferWindow>(PrefabNames.OfferWindow);
            for (var i = 0; i < ShopManager.OffersCount; i++)
            {
                var offerWindow = Instantiate(offerWindowPrefab, offerHolder);
                _spawnedOfferWindows.Add(offerWindow);
                offerWindow.SetShop(this);
                _uiElements.Add(offerWindow);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(offerHolder);
            
            InitBox();
        }

        private void Start()
        {
            refreshButton.SetShop(this);
            _uiElements.Add(refreshButton);
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
                _spawnedOfferWindows[i].SetOffer(offer, i);
            }
        }

        public void UpdateUI()
        {
            _uiElements.ForEach(e => e.OnUIUpdate());   
        }

        public void RefreshOffers()
        {
            PlayerCollectibleManager.ModifyCollectibleAmount(PlayerCollectibleManager.ECollectibleType.COIN, -ShopManager.RefreshCost);
            ShopManager.RefreshOffers(true);
            PopulateOfferWindows();
            UpdateUI();
        }
    }
}