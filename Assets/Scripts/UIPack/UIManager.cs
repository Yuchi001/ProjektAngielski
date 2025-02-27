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

        private readonly List<UIRecord> UIBaseList = new();

        private static UIManager Instance { get; set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        public static void OpenUI(IOpenStrategy openStrategy, ICloseStrategy closeStrategy)
        {
            var opened = openStrategy.Open(out var uiBase);
            if (!opened) return;
            
            Instance.UIBaseList.Add(new UIRecord(uiBase, closeStrategy));
        }

        public static void CloseUI(UIBase uiBase)
        {
            Instance.UIBaseList
                .FirstOrDefault(r => r.Script == uiBase)
                ?.CloseStrategy.Close();
        }

        public static UIBase SpawnUI(UIBase uiBase)
        {
            return Instantiate(uiBase, Instance.transform.position, Quaternion.identity, Instance.mainCanvas);
        }

        public static void RemoveUI(UIBase uiBase)
        {
            Instance.UIBaseList.RemoveAll(u => u.Script == uiBase);
        }

        public static IEnumerable<UIRecord> GetCurrentUIBaseList()
        {
            return Instance.UIBaseList;
        }

        public record UIRecord
        {
            public readonly UIBase Script;
            public readonly ICloseStrategy CloseStrategy;

            public UIRecord(UIBase script, ICloseStrategy closeStrategy)
            {
                CloseStrategy = closeStrategy;
                Script = script;
            }
        }
    }
}