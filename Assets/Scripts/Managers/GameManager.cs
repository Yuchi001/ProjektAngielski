using System;
using System.Collections.Generic;
using AudioPack;
using EffectPack.SO;
using EnchantmentPack;
using EnemyPack;
using ItemPack.WeaponPack.Other;
using MainCameraPack;
using Managers.Enums;
using Managers.Other;
using MapGeneratorPack;
using PlayerPack;
using PlayerPack.SO;
using UIPack;
using UnityEngine;
using UnityEngine.Serialization;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
       #region Singleton
       
       public static GameManager Instance { get; private set; }

       private void Awake()
       {
           if (Instance != this && Instance != null) Destroy(gameObject);
           else Instance = this;
       }
       
       #endregion
       
       public static KeyCode UpBind => KeyCode.W;
       public static KeyCode DownBind => KeyCode.S;
       public static KeyCode LeftBind => KeyCode.A;
       public static KeyCode RightBind => KeyCode.D;
       public static KeyCode AcceptBind => KeyCode.O;
       public static KeyCode DeclineBind => KeyCode.K;
       
       [SerializeField] private GameObject playerPrefab;
       [SerializeField] private MainCamera mainCamera;

       [SerializeField] private WaveManager waveManager;
       [SerializeField] private UIManager uiManager;

       [SerializeField] private SoCharacter baseCharacter; // TODO: usun

       private readonly Dictionary<string, GameObject> _prefabs = new();
       
       public PlayerManager CurrentPlayer { get; private set; }
       public WaveManager WaveManager => waveManager;
       public EnemySpawner EnemySpawner => waveManager.EnemySpawner;
       public int StageCount = 0;

       public delegate void StartRunDelegate();
       public static event StartRunDelegate OnStartRun;
       
       public delegate void InitDelegate();
       public static event InitDelegate OnInit;

       #region Cached

       private Projectile _projectilePrefab;
       public Projectile ProjectilePrefab => _projectilePrefab ??= GetPrefab<Projectile>(PrefabNames.Projectile);

       private IEnumerable<SoEffectBase> _effects = null;
       public IEnumerable<SoEffectBase> EffectList => _effects ??= Resources.LoadAll<SoEffectBase>("EffectStatus");

       private IEnumerable<SoEnchantment> _enchantments = null;
       public IEnumerable<SoEnchantment> EnchantmentList => _enchantments ??= Resources.LoadAll<SoEnchantment>("Enchantments");
       
       #endregion

       private void Start()
       {
           Init();
           StartRun(baseCharacter);
       }

       private void Init()
       {
           _prefabs.Clear();
           var prefabs = Resources.LoadAll<GameObject>("Prefabs");
           foreach (var prefab in prefabs)
           {
               _prefabs.Add(prefab.name, prefab);
           }
           
           LeanTween.init(100, 100);
           
           OnInit?.Invoke();
       }

       private void StartRun(SoCharacter soCharacter)
       {
           var playerObj = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
           CurrentPlayer = playerObj.GetComponent<PlayerManager>();
           CurrentPlayer.Setup(soCharacter);

           Time.timeScale = 1;
           
           OnStartRun?.Invoke();
       }

       public T GetPrefab<T>(string prefName) where T: class
       {
           var hasValue = _prefabs.TryGetValue(prefName, out var prefab);
           return hasValue ? prefab.GetComponent<T>() : null;
       }
    }
}