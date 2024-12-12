using System.Collections.Generic;
using AudioPack;
using EnemyPack;
using MainCameraPack;
using Managers.Enums;
using Managers.Other;
using MapGeneratorPack;
using PlayerPack;
using PlayerPack.SO;
using UnityEngine;

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

           Init();
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
       [SerializeField] private GameUiManager gameUiManager;

       [SerializeField] private SoCharacter baseCharacter; // TODO: usun

       private readonly Dictionary<string, GameObject> _prefabs = new();
       
       public PlayerManager CurrentPlayer { get; private set; }
       public MapGenerator MapGenerator { get; private set; }
       public WaveManager WaveManager => waveManager;
       public EnemySpawner EnemySpawner => waveManager.EnemySpawner;
       
       private void Init()
       {
           _prefabs.Clear();
           var prefabs = Resources.LoadAll<GameObject>("Prefabs");
           foreach (var prefab in prefabs)
           {
               _prefabs.Add(prefab.name, prefab);
           }
           
           LeanTween.init(100, 100);
           
           StartRun(baseCharacter);
       }

       public void StartRun(SoCharacter soCharacter)
       {
           MapGenerator = Instantiate(GetPrefab<MapGenerator>(PrefabNames.MapGeneratorManager));
           
           gameUiManager.BeginPlay();
           
           var playerObj = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
           CurrentPlayer = playerObj.GetComponent<PlayerManager>();
           CurrentPlayer.Setup(soCharacter);
           
           mainCamera.Setup(playerObj);
           
           AudioManager.SetTheme(EThemeType.Main1);
           
           waveManager.BeginSpawn();

           Time.timeScale = 1;
       }

       public T GetPrefab<T>(string prefName) where T: class
       {
           var hasValue = _prefabs.TryGetValue(prefName, out var prefab);
           return hasValue ? prefab.GetComponent<T>() : null;
       }
       
       public GameObject GetPrefab(string prefName)
       {
           var hasValue = _prefabs.TryGetValue(prefName, out var prefab);
           return hasValue ? prefab : null;
       }
    }
}