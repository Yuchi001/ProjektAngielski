using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EffectPack.SO;
using EnchantmentPack;
using ItemPack.WeaponPack.Other;
using MainCameraPack;
using Managers.Other;
using PlayerPack;
using PlayerPack.SO;
using SavePack;
using UIPack;
using UIPack.CloseStrategies;
using UIPack.OpenStrategies;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        // TODO: przenieś do PlayerMovement, zastąp nowym inputSystem
        public static KeyCode UpBind => KeyCode.W;
        public static KeyCode DownBind => KeyCode.S;
        public static KeyCode LeftBind => KeyCode.A;
        public static KeyCode RightBind => KeyCode.D;
        public static KeyCode AcceptBind => KeyCode.O;
        public static KeyCode DeclineBind => KeyCode.K;

        private readonly Dictionary<string, GameObject> _prefabs = new();
        public static bool HasInstance() => Instance != null;

        private int stageCount = 0;
        public static int StageCount => Instance.stageCount;

        public delegate void StartRunDelegate();
        public static event StartRunDelegate OnStartRun;

        public delegate void InitDelegate();
        public static event InitDelegate OnGMStart;

        private SaveManager.PlayerSaveData playerSaveData;

        private static GameManager Instance { get; set; }

        private static readonly string TRANSITION_UI_KEY;
        

        private void Awake()
        {
            if (Instance != this && Instance != null) Destroy(gameObject);
            else Instance = this;

            _prefabs.Clear();
            var prefabs = Resources.LoadAll<GameObject>("Prefabs");
            foreach (var prefab in prefabs) _prefabs.Add(prefab.name, prefab);

            LeanTween.init(100, 100);

            playerSaveData = SaveManager.LoadData();
            var allCharacters = Resources.LoadAll<SoCharacter>("Characters");
            var soCharacter = allCharacters.FirstOrDefault(e => e.ID == playerSaveData.pickedCharacterID) ?? allCharacters[0];
            
            SceneManager.LoadScene((int)EScene.TAVERN, LoadSceneMode.Additive);
            var isMenu = SceneExtensions.IsSceneLoaded((int)EScene.MENU);
            var playerPrefab = GetPrefab<PlayerManager>(PrefabNames.GamePlayer);
            Instantiate(playerPrefab, Vector3.zero, Quaternion.identity)
                .Setup(soCharacter, isMenu ? PlayerManager.State.IN_MENU : PlayerManager.State.IN_TAVERN);
            
            SaveManager.LoadData();
        }

        private void Start()
        {
            OnGMStart?.Invoke();
        }

        public static void StartRun()
        {
            MainCamera.InOutAnim(0.3f, () =>
            {
                SceneManager.UnloadSceneAsync((int)EScene.TAVERN);
            }, () =>
            {
                OnStartRun?.Invoke();
                PlayerManager.SetPlayerState(PlayerManager.State.ON_MISSION);
            });
        }
        
        public static void LoadGameScene(LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            SceneManager.LoadScene((int)EScene.MAIN_GAME, loadSceneMode);
        }

        public static void LoadMenu()
        {
            SceneManager.LoadScene((int)EScene.MENU);
        }
        
        public static void GoToTavern()
        {
            SceneManager.UnloadSceneAsync((int)EScene.MENU);
            TavernManager.LoadTavern();
            PlayerManager.SetPlayerState(PlayerManager.State.IN_TAVERN);
        }

        public static T GetPrefab<T>(string prefName) where T : class
        {
            var hasValue = Instance._prefabs.TryGetValue(prefName, out var prefab);
            return hasValue ? prefab.GetComponent<T>() : null;
        }

        private void OnApplicationQuit()
        {
            SaveManager.SaveData();
        }

        private enum EScene
        {
            MENU = 0,
            TAVERN = 1,
            MAIN_GAME = 2,
        }
    }
}