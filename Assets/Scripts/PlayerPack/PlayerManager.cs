using System;
using System.Collections.Generic;
using Managers;
using Managers.Other;
using PlayerPack.SO;
using UIPack;
using UIPack.CloseStrategies;
using UIPack.OpenStrategies;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace PlayerPack
{
    public class PlayerManager : MonoBehaviour
    {
        #region Singleton
        
        private static PlayerManager Instance { get; set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        #endregion

        [SerializeField] private SpriteRenderer playerSpriteRenderer;
        [SerializeField] private Animator animator;

        private SoCharacter _pickedCharacter;
        public static SoCharacter PickedCharacter => Instance._pickedCharacter;

        public static bool HasInstance() => Instance != null;
        public static Transform GetTransform() => Instance.transform;
        public static bool CanInteract() => PlayerInput.actions.enabled;


        private PlayerInput _playerInput;
        public static PlayerInput PlayerInput
        {
            get
            {
                if (Instance._playerInput == null) Instance._playerInput = Instance.GetComponent<PlayerInput>();
                return Instance._playerInput;
            }
        }
        
        private PlayerHealth _playerHealth;
        public static PlayerHealth PlayerHealth
        {
            get
            {
                if (Instance._playerHealth == null) Instance._playerHealth = Instance.GetComponent<PlayerHealth>();
                return Instance._playerHealth;
            }
        }

        private PlayerSoulManager _playerSoulManager;
        public static PlayerSoulManager PlayerSoulManager
        {
            get
            {
                if (Instance._playerSoulManager == null) Instance._playerSoulManager = Instance.GetComponent<PlayerSoulManager>();
                return Instance._playerSoulManager;
            }
        }

        private PlayerItemManager _playerItemManager;
        public static PlayerItemManager PlayerItemManager => Instance._playerItemManager;
        
        private PlayerUIManager _playerUIManager;
        public static PlayerUIManager PlayerUIManager => Instance._playerUIManager;

        private PlayerMovement _playerMovement;
        public static PlayerMovement PlayerMovement {
            get
            {
                if (Instance._playerMovement == null) Instance._playerMovement = Instance.GetComponent<PlayerMovement>();
                return Instance._playerMovement;
            }
        }

        private PlayerEnchantments _playerEnchantments;
        public static PlayerEnchantments PlayerEnchantments
        {
            get
            {
                if (Instance._playerEnchantments == null) Instance._playerEnchantments = Instance.GetComponent<PlayerEnchantments>();
                return Instance._playerEnchantments;
            }
        }

        private PlayerStatsManager _playerStatsManager;
        public static PlayerStatsManager PlayerStatsManager {
            get
            {
                if (Instance._playerStatsManager == null) Instance._playerStatsManager = Instance.GetComponent<PlayerStatsManager>();
                return Instance._playerStatsManager;
            }
        }

        private PlayerCoinManager _playerCoinManager;
        public static PlayerCoinManager PlayerCoinManager
        {
            get
            {
                if (Instance._playerCoinManager == null) Instance._playerCoinManager = Instance.GetComponent<PlayerCoinManager>();
                return Instance._playerCoinManager;
            }
        }

        public static Vector2 PlayerPos => Instance.transform.position;
        
        public delegate void PlayerDeathDelegate();
        public static event PlayerDeathDelegate OnPlayerDeath;
        
        public delegate void PlayerReadyDelegate();
        public static event PlayerReadyDelegate OnPlayerReady;

        public delegate void ChangeStateDelegate(State newState);
        public static event ChangeStateDelegate OnChangeState;

        private State _currentState;
        public static State CurrentState => Instance._currentState;

        public void Setup(SoCharacter pickedCharacter, State defaultState)
        {
            var playerItemManagerPrefab = GameManager.GetPrefab<PlayerItemManager>(PrefabNames.PlayerInventoryManager);
            var openStrategy = new SingletonOpenStrategy<PlayerItemManager>(playerItemManagerPrefab);
            var closeStrategy = new DefaultCloseStrategy();
            _playerItemManager = UIManager.OpenUI<PlayerItemManager>("PlayerItemManager", openStrategy, closeStrategy);
            _playerUIManager = _playerItemManager.GetComponent<PlayerUIManager>();
            
            ChangeCharacter(pickedCharacter, true);
            foreach (var mono in Instance.GetComponentsInChildren<MonoBehaviour>())
                mono.enabled = true;
            
            SetPlayerState(defaultState);
            
            OnPlayerReady?.Invoke();
        }

        public static void SetPosition(Vector2 position)
        {
            Instance.transform.position = position;
            Instance.transform.AdjustForPivot(Instance.playerSpriteRenderer);
        }

        public static void ChangeCharacter(SoCharacter newCharacter, bool init)
        {
            Instance._pickedCharacter = newCharacter;
            Instance.playerSpriteRenderer.sprite = PickedCharacter.CharacterSprite;
            PlayerStatsManager.SetCharacter(newCharacter);

            Instance.animator.SetCharacterAnimations(newCharacter);
            PlayerItemManager.CleanInventory();
            PlayerItemManager.AddItem(PickedCharacter.StartingItem, 1);

            if (init) return;
            PlayerUIManager.PrepareUI();
        }
        
        public static void SetPlayerState(State state)
        {
            Instance._currentState = state;
            switch (state)
            {
                case State.IN_TAVERN:
                    PlayerItemManager.DestroyAllItems();
                    UnlockKeys();
                    break;
                case State.ON_MISSION:
                    PlayerItemManager.RefreshInventory();
                    UnlockKeys();
                    break;
                case State.IN_MENU:
                    PlayerItemManager.DestroyAllItems();
                    LockKeys();
                    break;
                case State.IN_MAP:
                    PlayerItemManager.DestroyAllItems();
                    UnlockKeys();
                    break;
                default:
                    throw new Exception($"State {state} not implemented in PlayerManager.SetState");
            }

            OnChangeState?.Invoke(CurrentState);
        }

        public static void LockKeys()
        {
            PlayerInput.actions.Disable();
        }

        public static void UnlockKeys()
        {
            PlayerInput.actions.Enable();
        }

        public static void ManagePlayerDeath()
        {
            PlayerHealth.enabled = false;
            
            PlayerItemManager.DestroyAllItems();
            PlayerItemManager.enabled = false;

            OnPlayerDeath?.Invoke();
        }

        public enum State
        {
            IN_TAVERN,
            ON_MISSION,
            IN_MENU,
            IN_MAP,
        }
    }
}