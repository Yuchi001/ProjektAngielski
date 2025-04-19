using System;
using System.Collections.Generic;
using System.Linq;
using AccessorPack;
using ItemPack;
using Managers;
using Managers.Enums;
using Managers.Other;
using Other;
using Other.Enums;
using PlayerPack.Decorators;
using PlayerPack.Interface;
using PlayerPack.PlayerEnchantmentPack;
using PlayerPack.PlayerItemPack;
using PlayerPack.SO;
using UIPack;
using UIPack.CloseStrategies;
using UIPack.OpenStrategies;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace PlayerPack
{
    public class PlayerManager : MonoBehaviour, IDamageContextManager, IEffectContextManager, IHealContextManager
    {
        #region Singleton
        
        private static PlayerManager Instance { get; set; }
        public static IDamageContextManager GetDamageContextManager() => Instance;
        public static IEffectContextManager GetEffectContextManager() => Instance;
        public static IHealContextManager GetHealContextManager() => Instance;

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

        private readonly List<KeyValuePair<string, IDamageModifier>> _damageModifiers = new();
        private readonly List<KeyValuePair<string, IEffectModifier>> _effectModifiers = new();
        private readonly List<KeyValuePair<string, IHealModifier>> _healModifiers = new();


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
        
        private PlayerCollectibleManager _playerCollectible;
        public static PlayerCollectibleManager PlayerCollectibleManager
        {
            get
            {
                if (Instance._playerCollectible == null) Instance._playerCollectible = Instance.GetComponent<PlayerCollectibleManager>();
                return Instance._playerCollectible;
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
            var playerItemManagerOpenStrategy = new SingletonOpenStrategy<PlayerItemManager>(playerItemManagerPrefab);
            var playerItemManagerCloseStrategy = new DefaultCloseStrategy();
            _playerItemManager = UIManager.OpenUI<PlayerItemManager>("PlayerItemManager", playerItemManagerOpenStrategy, playerItemManagerCloseStrategy);
            _playerUIManager = _playerItemManager.GetComponent<PlayerUIManager>();
            
            var playerEnchantmentsPrefab = GameManager.GetPrefab<PlayerEnchantmentUI>(PrefabNames.PlayerEnchantmentUI);
            var playerEnchantmentsOpenStrategy = new SingletonOpenStrategy<PlayerEnchantmentUI>(playerEnchantmentsPrefab);
            var playerEnchantmentsCloseStrategy = new DefaultCloseStrategy();
            UIManager.OpenUI<PlayerEnchantmentUI>("PlayerEnchantments", playerEnchantmentsOpenStrategy, playerEnchantmentsCloseStrategy);
            
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

        public static void StartPlayerExitSequence(Action rewardPlayerAction)
        {
            //TODO: portal open audio
            MainSceneAccessor.EnemySpawner.SetState(ESpawnerState.Stop);
            rewardPlayerAction.Invoke();
        }

        public static void ManagePlayerDeath()
        {
            PlayerHealth.enabled = false;
            
            PlayerItemManager.DestroyAllItems();
            PlayerItemManager.enabled = false;

            OnPlayerDeath?.Invoke();
        }

        public void AddDamageModifier(string key, IDamageModifier modifier)
        {
            _damageModifiers.Add(new KeyValuePair<string, IDamageModifier>(key, modifier));
            _damageModifiers.Sort((a, b) => a.Value.QueueAsLast.CompareTo(b.Value.QueueAsLast));
        }

        public void RemoveDamageModifier(string key)
        {
            _damageModifiers.RemoveAll(m => m.Key == key);
        }
        
        public DamageContext GetDamageContext(int damage, ItemLogicBase source)
        {
            var context = new DamageContext(damage, source.InventoryItem.ItemTags, source);
            foreach (var pair in _damageModifiers)
            {
                pair.Value.ModifyDamageContext(context);
            }

            return context;
        }

        public void AddEffectModifier(string key, IEffectModifier modifier)
        {
            _effectModifiers.Add(new KeyValuePair<string, IEffectModifier>(key, modifier));
        }

        public void RemoveEffectModifier(string key)
        {
            _effectModifiers.RemoveAll(m => m.Key == key);
        }

        public EffectContext GetEffectContext(EEffectType effectType, float duration, CanBeDamaged canBeDamaged)
        {
            var context = new EffectContext(effectType, duration, canBeDamaged);
            foreach (var pair in _effectModifiers)
            {
                pair.Value.ModifyEffectContext(context);
            }

            return context;
        }
        
        public void AddHealModifier(string key, IHealModifier modifier)
        {
            _healModifiers.Add(new KeyValuePair<string, IHealModifier>(key, modifier));
        }

        public void RemoveHealModifier(string key)
        {
            _healModifiers.RemoveAll(m => m.Key == key);
        }

        public HealContext GetHealContext(int value, CanBeDamaged canBeDamaged)
        {
            var context = new HealContext(value, canBeDamaged);
            foreach (var pair in _healModifiers)
            {
                pair.Value.ModifyHealContext(context);
            }

            return context;
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