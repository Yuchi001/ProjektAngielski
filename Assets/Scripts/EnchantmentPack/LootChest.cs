using System;
using System.Collections;
using System.Linq;
using AudioPack;
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
    // TODO: uzupelnij klase
    public class LootChest : MonoBehaviour
    {
        [SerializeField] private SoEnemy mimic;
        [SerializeField] private GameObject chestPopup;
        [SerializeField] private GameObject lightObj;
        [SerializeField] private Sprite openedChestSprite;
        [SerializeField, Tooltip("In Seconds")] private float openedChestLifeTime = 10;
        private SpriteRenderer SpriteRenderer => GetComponent<SpriteRenderer>();
        private EnemySpawner EnemySpawner => GameManager.Instance.WaveManager.EnemySpawner;
        
        private bool _opened = false;
        private bool _inRange = false;
        
        private void Open()
        {
            AudioManager.Instance.PlaySound(ESoundType.Chest);
            _opened = true;
            chestPopup.SetActive(false);
            lightObj.SetActive(false);
            SpriteRenderer.sprite = openedChestSprite;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            _inRange = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            _inRange = false;
        }

        private void Update()
        {
            if (!_inRange || PlayerManager.Instance == null || _opened) return;
            
            if (!Input.GetKeyDown(GameManager.AcceptBind)) return;

            // MIMIC SPAWN
            /*if (enchant == null)
            {
                EnemySpawner.SpawnEnemy(mimic, transform.position);
                Destroy(gameObject);
                return;
            }*/
            
            Destroy(gameObject, openedChestLifeTime);
        }
    }
}