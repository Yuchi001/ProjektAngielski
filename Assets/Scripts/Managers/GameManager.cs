using System;
using System.Collections;
using System.Collections.Generic;
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

            SceneManager.LoadScene((int)EScene.TAVERN);
        }

        private void Start()
        {
            OnGMStart?.Invoke();
        }

        public static void StartRun(SoCharacter soCharacter)
        {
            var asyncOperation = SceneManager.UnloadSceneAsync((int)EScene.TAVERN);
            var playerPrefab = GetPrefab<PlayerManager>(PrefabNames.GamePlayer);
            Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).Setup(soCharacter).LockKeys();
            var transitionUIPrefab = GetPrefab<TransitionUI>(PrefabNames.TransitionUI);
            var openStrat = new DefaultOpenStrategy(transitionUIPrefab);
            var closeStrat = new DestroyCloseStrategy(TRANSITION_UI_KEY);
            var openedUI = UIManager.OpenUI<TransitionUI>(TRANSITION_UI_KEY, openStrat, closeStrat);
            Instance.StartCoroutine(StartRunAsync(asyncOperation, openedUI));
        }

        private static IEnumerator StartRunAsync(AsyncOperation asyncOperation, TransitionUI transitionUI)
        {
            while (!asyncOperation.isDone)
            {
                transitionUI.SetLoadingProgress(asyncOperation.progress);
                yield return new WaitForEndOfFrame();
            }
            transitionUI.SetLoadingProgress(1);

            OnStartRun?.Invoke();
            UIManager.CloseUI(TRANSITION_UI_KEY);
            yield return new WaitForSeconds(0.3f);
            PlayerManager.Instance.UnlockKeys();
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