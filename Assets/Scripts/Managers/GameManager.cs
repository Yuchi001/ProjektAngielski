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
       
       [SerializeField] private SoCharacter baseCharacter; // TODO: usun

       private readonly Dictionary<string, GameObject> _prefabs = new();

       public int StageCount = 0;

       public delegate void StartRunDelegate();
       public static event StartRunDelegate OnStartRun;
       
       public delegate void InitDelegate();
       public static event InitDelegate OnInit;

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
           var playerPrefab = GetPrefab<PlayerManager>(PrefabNames.GamePlayer);
           Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).Setup(soCharacter);

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