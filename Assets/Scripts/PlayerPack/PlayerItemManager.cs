 using System.Collections.Generic;
using System.Linq;
using ItemPack;
using ItemPack.SO;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlayerPack
{
    public class PlayerItemManager : MonoBehaviour
    {
        private readonly List<ItemLogicBase> _currentItems = new();
        private List<SoItem> _allItems = new();

        public delegate void WeaponAddDelegate(ItemLogicBase itemLogicBase);
        public static event WeaponAddDelegate OnItemAdd;

        
        private void Awake()
        {
            _allItems = Resources.LoadAll<SoItem>("Weapons").Select(Instantiate).ToList();
        }

        public void AddItem(SoItem itemToAdd, int level)
        {
            var itemLogicObj = Instantiate(itemToAdd.ItemPrefab, transform, true);
            itemLogicObj.transform.localPosition = Vector3.zero;
            var itemLogic = itemLogicObj.GetComponent<ItemLogicBase>();
            itemLogic.Setup(itemToAdd);
            _currentItems.Add(itemLogic);
            
            OnItemAdd?.Invoke(itemLogic);
            
            var availableWeapons = _currentItems.Where(w => w.Item.ItemName == itemToAdd.ItemName && w.Level == level);
            if (availableWeapons.Count() < StaticOptions.LEVEL_UP_PER_WEAPONS_COUNT || level >= StaticOptions.MAX_TIER) return;

            for (var i = 0; i < StaticOptions.LEVEL_UP_PER_WEAPONS_COUNT; i++) 
                DestroyItem(itemToAdd.ItemName);
            
            AddItem(itemToAdd, level + 1);
        }

        public IEnumerable<SoItem> GetRandomWeapons(int count)
        {
            var weapons = new List<SoItem>();
            var weaponPool = new List<SoItem>(_allItems);

            for (var i = 0; i < count; i++)
            {
                if (weaponPool.Count == 0) break;
                
                var randomIndex = Random.Range(0, weaponPool.Count);
                var pickedWeapon = Instantiate(weaponPool[randomIndex]);
                weapons.Add(pickedWeapon);
                weaponPool.RemoveAt(randomIndex);
            }
            
            return weapons;
        }

        public void DestroyAllItems()
        {
            foreach (var item in _currentItems.Where(i => i != null))
            {
                Destroy(item.gameObject);
            }
        }

        public void DestroyItem(string itemName)
        {
            var item = _currentItems.FirstOrDefault(i => i != null && i.Item.ItemName == itemName);
            if (item == default) return;
            
            _currentItems.Remove(item);
            Destroy(item.gameObject);
        }
    }
}