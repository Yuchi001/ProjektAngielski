using EnemyPack;
using MainCameraPack;
using Managers.Enums;
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
       
       #region Prefabs

       [Header("Public prefabs")] 
       [SerializeField] private GameObject spawnEntityPrefab;
       [SerializeField] private GameObject mapGenerator;
       public GameObject SpawnEntityPrefab => spawnEntityPrefab;

       #endregion
       public PlayerManager CurrentPlayer { get; private set; }
       public MapGenerator MapGenerator { get; private set; }
       public WaveManager WaveManager => waveManager;

       private void Init()
       {
           LeanTween.init(1000000, 1000000);
       }

       public void StartRun(SoCharacter pickedCharacter)
       {
           MapGenerator = Instantiate(mapGenerator).GetComponent<MapGenerator>();
           
           gameUiManager.BeginPlay();
           
           var playerObj = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
           CurrentPlayer = playerObj.GetComponent<PlayerManager>();
           CurrentPlayer.Setup(pickedCharacter);
           
           mainCamera.Setup(playerObj);
           
           AudioManager.Instance.SetTheme(EThemeType.Main1);
           
           waveManager.BeginSpawn();

           Time.timeScale = 1;
       }
    }
}