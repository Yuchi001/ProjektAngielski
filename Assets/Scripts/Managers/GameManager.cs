using System.Collections.Generic;
using EffectPack.SO;
using EnchantmentPack;
using ItemPack.WeaponPack.Other;
using MainCameraPack;
using Managers.Other;
using PlayerPack;
using PlayerPack.SO;
using UIPack;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameManager : MonoBehaviour
    { 
        public static KeyCode UpBind => KeyCode.W;
       public static KeyCode DownBind => KeyCode.S;
       public static KeyCode LeftBind => KeyCode.A;
       public static KeyCode RightBind => KeyCode.D;
       public static KeyCode AcceptBind => KeyCode.O;
       public static KeyCode DeclineBind => KeyCode.K;
       
       private readonly Dictionary<string, GameObject> _prefabs = new();

       private int stageCount = 0;
       public static int StageCount => Instance.stageCount;

       public delegate void StartRunDelegate();
       public static event StartRunDelegate OnStartRun;
       
       public delegate void InitDelegate();
       public static event InitDelegate OnGMStart;
       
       private static GameManager Instance { get; set; }

       private void Awake()
       {
           if (Instance != this && Instance != null) Destroy(gameObject);
           else Instance = this;
           
           _prefabs.Clear();
           var prefabs = Resources.LoadAll<GameObject>("Prefabs");
           foreach (var prefab in prefabs)
           {
               _prefabs.Add(prefab.name, prefab);
           }
           
           LeanTween.init(100, 100);
       }

       private void Start()
       {
           OnGMStart?.Invoke();
       }

       public static void StartRun(SoCharacter soCharacter)
       {
           SceneManager.LoadScene((int)EScene.MAIN_GAME);
           
           var playerPrefab = GetPrefab<PlayerManager>(PrefabNames.GamePlayer);
           Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).Setup(soCharacter);

           Time.timeScale = 1;
           
           OnStartRun?.Invoke();
       }

       public static T GetPrefab<T>(string prefName) where T: class
       {
           var hasValue = Instance._prefabs.TryGetValue(prefName, out var prefab);
           return hasValue ? prefab.GetComponent<T>() : null;
       }

       private enum EScene
       {
           MENU = 0,
           TAVERN = 1,
           MAIN_GAME = 2,
       }
    }
}