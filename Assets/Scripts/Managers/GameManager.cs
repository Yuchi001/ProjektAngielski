using MainCameraPack;
using Managers.Enums;
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
       
       [SerializeField] private GameObject playerPrefab;
       [SerializeField] private MainCamera mainCamera;

       [SerializeField] private WaveManager waveManager;
       [SerializeField] private GameUiManager gameUiManager;

       [SerializeField] private float boundaryX;
       [SerializeField] private float boundaryY;

       #region Prefabs

       [Header("Public prefabs")] 
       [SerializeField] private GameObject spawnEntityPrefab;
       public GameObject SpawnEntityPrefab => spawnEntityPrefab;

       #endregion
       public PlayerManager CurrentPlayer { get; private set; }
       public float BoundaryX => boundaryX;
       public float BoundaryY => boundaryY;

       private void Init()
       {
           LeanTween.init(1000000, 1000000);
       }

       public void StartRun(SoCharacter pickedCharacter)
       {
           gameUiManager.BeginPlay();
           
           var playerObj = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
           CurrentPlayer = playerObj.GetComponent<PlayerManager>();
           CurrentPlayer.Setup(pickedCharacter);
           
           mainCamera.Setup(playerObj);
           
           AudioManager.Instance.SetTheme(EThemeType.Main1);
           
           waveManager.BeginSpawn();
       }
    }
}