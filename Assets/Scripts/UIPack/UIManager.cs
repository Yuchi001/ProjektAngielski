using System.Collections.Generic;
using System.Linq;
using UIPack.CloseStrategies;
using UIPack.OpenStrategies;
using UnityEngine;

namespace UIPack
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Transform mainCanvas;
        [SerializeField] private Transform worldCanvas;

        private readonly List<UIRecord> UIBaseList = new();

        public static Transform WorldCanvas => Instance.worldCanvas;

        private static UIManager Instance { get; set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;

            UIBaseList.Clear();
        }

        public static T OpenUI<T>(string key, IOpenStrategy openStrategy, ICloseStrategy closeStrategy) where T: UIBase
        {
            var opened = openStrategy.Open(out var uiBase, key);
            if (!opened) return null;

            AddUI(key, closeStrategy, uiBase);
            return uiBase as T;
        }

        public static void CloseUI(string key, bool removeFromList = false)
        {
            var record = Instance.UIBaseList.FirstOrDefault(r => r.Key == key);
            if (record == default || !record.Script.Open) return;
            
            record.CloseStrategy.Close(record.Script);
            if (removeFromList) RemoveUI(key);
        }

        public static bool IsOpen(string key)
        {
            return Instance.UIBaseList.FirstOrDefault(e => e.Key == key && e.Script.Open) != default;
        }

        public static void CloseAllUIs()
        {
            foreach (var record in Instance.UIBaseList.Where(record => record.Script.Open))
            {
                record.CloseStrategy.Close(record.Script);
                RemoveUI(record.Key);
            }
        }

        public static UIBase SpawnUI(UIBase uiBase)
        {
            return Instantiate(uiBase, Instance.mainCanvas);
        }

        public void ChangeToWorldPos(string key, Vector2 position)
        {
            var uiTransform = Instance.UIBaseList.First(r => r.Key == key).Script.transform;
            uiTransform.SetParent(worldCanvas);
            uiTransform.position = position;
        }

        private static void AddUI(string key, ICloseStrategy closeStrategy, UIBase spawnedUIBase)
        {
            var alreadyInList = Instance.UIBaseList.FirstOrDefault(r => r.Key == key) != default;
            if (alreadyInList) return;
            Instance.UIBaseList.Add(new UIRecord(key, spawnedUIBase, closeStrategy));
        }

        public static void RemoveUI(string key)
        {
            Instance.UIBaseList.RemoveAll(u => u.Key == key);
        }

        public static IEnumerable<UIRecord> GetCurrentUIBaseList()
        {
            return Instance.UIBaseList;
        }

        public record UIRecord
        {
            public readonly string Key;
            public readonly UIBase Script;
            public readonly ICloseStrategy CloseStrategy;

            public UIRecord(string key, UIBase script, ICloseStrategy closeStrategy)
            {
                Key = key;
                CloseStrategy = closeStrategy;
                Script = script;
            }
        }
    }
}