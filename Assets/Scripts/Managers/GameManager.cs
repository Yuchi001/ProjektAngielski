using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AudioPack;
using GameLoaderPack;
using MainCameraPack;
using Managers.Other;
using MapPack;
using PlayerPack;
using PlayerPack.SO;
using SavePack;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        // TODO: przenieś do PlayerMovement, zastąp nowym inputSystem
        public static KeyCode DeclineBind => KeyCode.K;

        private readonly Dictionary<string, GameObject> _prefabs = new();
        public static bool HasInstance() => Instance != null;

        private int stageCount = 0;
        public static int StageCount => Instance.stageCount;

        public delegate void InitDelegate();
        public static event InitDelegate OnGMStart;

        private SaveManager.PlayerSaveData playerSaveData;

        private List<MapManager.MissionData> _currentMissions = new();
        public static List<MapManager.MissionData> GetMissions() => Instance._currentMissions;
        public static void SetMissions(List<MapManager.MissionData> missions) => Instance._currentMissions = missions;

        private static GameManager Instance { get; set; }

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

        public static void StartRun(MapManager.MissionData missionData)
        {
            MainCamera.InOutAnim(0.3f, () =>
            {
                SceneManager.UnloadSceneAsync((int)EScene.MAP);
                MainCamera.SetSize(4);
            }, () =>
            {
                AudioManager.SetTheme(missionData.ThemeType);
                PlayerManager.SetPlayerState(PlayerManager.State.ON_MISSION);
            }, DelayedLoadCondition(missionData));
        }

        private static IEnumerator DelayedLoadCondition(MapManager.MissionData missionData)
        {
            var asyncOperation = SceneManager.LoadSceneAsync((int)EScene.GAME, LoadSceneMode.Additive);
            return new WaitUntil(() => asyncOperation.isDone && GameLoader.HasInstance() && GameLoader.LoadScene(missionData));
        }

        public static void OpenMap()
        {
            PlayerManager.LockKeys();
            MainCamera.InOutAnim(0.3f, () =>
            {
                SceneManager.UnloadSceneAsync((int)EScene.TAVERN);
                SceneManager.LoadSceneAsync((int)EScene.MAP, LoadSceneMode.Additive);
            }, () =>
            {
                PlayerManager.SetPlayerState(PlayerManager.State.IN_MAP);
                PlayerManager.UnlockKeys();
            });
        }

        public static void ReturnToMap()
        {
            PlayerManager.LockKeys();
            MainCamera.InOutAnim(0.3f, () =>
            {
                SceneManager.UnloadSceneAsync((int)EScene.GAME);
                SceneManager.LoadSceneAsync((int)EScene.MAP);
            }, () =>
            {
                PlayerManager.SetPlayerState(PlayerManager.State.IN_MAP);
                PlayerManager.UnlockKeys();
            });
        }
        
        public static void LoadGameScene(LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            SceneManager.LoadScene((int)EScene.MAIN, loadSceneMode);
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

        public enum EScene
        {
            MENU = 0,
            TAVERN = 1,
            MAIN = 2,
            MAP = 3,
            GAME = 4
        }
    }
}