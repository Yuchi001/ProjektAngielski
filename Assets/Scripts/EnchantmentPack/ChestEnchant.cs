using System;
using System.Collections;
using System.Linq;
using EnemyPack;
using EnemyPack.CustomEnemyLogic;
using EnemyPack.SO;
using Managers;
using Managers.Enums;
using PlayerPack;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace EnchantmentPack
{
    public class ChestEnchant : MonoBehaviour
    {
        [SerializeField] private SoEnemy mimic;
        [SerializeField] private float openRange = 0.5f;
        [SerializeField] private GameObject chestPopup;
        [SerializeField] private GameObject lightObj;
        [SerializeField] private GameObject enchantmentMenuPrefab;
        [SerializeField] private Sprite openedChestSprite;
        [SerializeField] private GameObject confettiParticles;
        [SerializeField, Tooltip("In Seconds")] private float openedChestLifeTime = 10;
        private static Vector2 PlayerPos => PlayerManager.Instance.transform.position;
        private SpriteRenderer SpriteRenderer => GetComponent<SpriteRenderer>();
        private EnemySpawner EnemySpawner => GameManager.Instance.WaveManager.EnemySpawner;
        
        private bool _opened = false;

        private SoEnchantment _preparedEnchantment;

        public void Setup(SoEnchantment enchantment)
        {
            _preparedEnchantment = enchantment;
        }
        
        private SoEnchantment Open()
        {
            AudioManager.Instance.PlaySound(ESoundType.Chest);
            _opened = true;
            chestPopup.SetActive(false);
            lightObj.SetActive(false);
            SpriteRenderer.sprite = openedChestSprite;

            if (_preparedEnchantment != null) return _preparedEnchantment;
            
            var list = PlayerManager.Instance.PlayerEnchantmentManager.GetRandomEnchantmentList(1);

            var soEnchantments = list as SoEnchantment[] ?? list.ToArray();
            return !soEnchantments.Any() ? null : soEnchantments[0];
        }

        private void Update()
        {
            if (_opened) return;
            
            if (PlayerManager.Instance == null) return;

            var inDistance = Vector2.Distance(PlayerPos, transform.position) <= openRange;
            chestPopup.SetActive(inDistance);
            if (!inDistance) return;
            
            if (!Input.GetKeyDown(GameManager.AcceptBind)) return;
            
            var enchant = Open();
            if (enchant == null)
            {
                EnemySpawner.SpawnEnemy(mimic, transform.position);
                Destroy(gameObject);
                return;
            }
            
            Destroy(gameObject, openedChestLifeTime);
            Time.timeScale = 0;
            
            var enchantmentMenuObj = Instantiate(enchantmentMenuPrefab, GameUiManager.Instance.GameCanvas, false);
            enchantmentMenuObj.GetComponent<EnchantmentUI>().Setup(enchant);

            var particles = Instantiate(confettiParticles, transform.position, quaternion.identity);
            Destroy(particles, 5);
        }
    }
}