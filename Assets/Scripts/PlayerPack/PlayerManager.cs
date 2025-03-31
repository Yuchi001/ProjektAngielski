using System.Collections.Generic;
using Managers;
using Managers.Other;
using PlayerPack.PlayerMovementPack;
using PlayerPack.SO;
using UIPack;
using UIPack.CloseStrategies;
using UIPack.OpenStrategies;
using UnityEngine;

namespace PlayerPack
{
    public class PlayerManager : MonoBehaviour
    {
        #region Singleton
        
        public static PlayerManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        #endregion

        [SerializeField] private SpriteRenderer playerSpriteRenderer;
        [SerializeField] private Animator animator;

        public SoCharacter PickedCharacter { get; private set; }

        private PlayerHealth _playerHealth;
        public PlayerHealth PlayerHealth
        {
            get
            {
                if (_playerHealth == null) _playerHealth = GetComponent<PlayerHealth>();
                return _playerHealth;
            }
        }

        private PlayerSoulManager _playerSoulManager;
        public PlayerSoulManager PlayerSoulManager
        {
            get
            {
                if (_playerSoulManager == null) _playerSoulManager = GetComponent<PlayerSoulManager>();
                return _playerSoulManager;
            }
        }

        public PlayerItemManager PlayerItemManager { get; private set; }

        private PlayerMovement _playerMovement;
        public PlayerMovement PlayerMovement {
            get
            {
                if (_playerMovement == null) _playerMovement = GetComponent<PlayerMovement>();
                return _playerMovement;
            }
        }

        private PlayerEnchantments _playerEnchantments;
        public PlayerEnchantments PlayerEnchantments
        {
            get
            {
                if (_playerEnchantments == null) _playerEnchantments = GetComponent<PlayerEnchantments>();
                return _playerEnchantments;
            }
        }

        private PlayerStatsManager _playerStatsManager;
        public PlayerStatsManager PlayerStatsManager {
            get
            {
                if (_playerStatsManager == null) _playerStatsManager = GetComponent<PlayerStatsManager>();
                return _playerStatsManager;
            }
        }

        private PlayerCoinManager _playerCoinManager;
        public PlayerCoinManager PlayerCoinManager
        {
            get
            {
                if (_playerCoinManager == null) _playerCoinManager = GetComponent<PlayerCoinManager>();
                return _playerCoinManager;
            }
        }

        public Vector2 PlayerPos => transform.position;
        
        public delegate void PlayerDeathDelegate();
        public static event PlayerDeathDelegate OnPlayerDeath;
        
        public delegate void PlayerReadyDelegate();
        public static event PlayerReadyDelegate OnPlayerReady;

        public void Setup(SoCharacter pickedCharacter)
        {
            PickedCharacter = pickedCharacter;
            playerSpriteRenderer.sprite = PickedCharacter.CharacterSprite;

            foreach (var mono in GetComponentsInChildren<MonoBehaviour>())
                mono.enabled = true;
            
            var aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);
            var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            foreach (var a in aoc.animationClips)
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, a.name == "DwarfAIdle" ? 
                    PickedCharacter.IdleAnimation : 
                    PickedCharacter.WalkingAnimation));
            aoc.ApplyOverrides(anims);
            animator.runtimeAnimatorController = aoc;
            
            var playerItemManagerPrefab = GameManager.GetPrefab<PlayerItemManager>(PrefabNames.PlayerInventoryManager);
            var openStrategy = new SingletonOpenStrategy<PlayerItemManager>(playerItemManagerPrefab);
            var closeStrategy = new DefaultCloseStrategy();
            PlayerItemManager = UIManager.OpenUI<PlayerItemManager>("PlayerItemManager", openStrategy, closeStrategy);
            PlayerItemManager.AddItem(pickedCharacter.StartingItem, 1);
            
            OnPlayerReady?.Invoke();
        }

        public void ManagePlayerDeath()
        {
            PlayerHealth.enabled = false;
            
            PlayerItemManager.DestroyAllItems();
            PlayerItemManager.enabled = false;

            OnPlayerDeath?.Invoke();
        }
    }
}