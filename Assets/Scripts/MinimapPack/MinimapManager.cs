using System.Collections.Generic;
using Managers;
using Managers.Other;
using UIPack;
using UnityEngine;
using UnityEngine.UI;

namespace MinimapPack
{
    public class MinimapManager : UIBase
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image worldImage;   
        [SerializeField] private int minimapSize;
        [SerializeField] private int iconSize = 8;

        private float scale;
        public static float MinimapScale => Instance.scale;
        public static int IconSize => Instance.iconSize;

        private readonly Dictionary<string, MinimapElement> _minimapElements = new();
        
        private static MinimapManager Instance { get; set; }
        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        public void SetupMinimap(Vector2Int worldSize)
        {
            foreach (var minimapElement in Instance._minimapElements.Values)
            {
                Destroy(minimapElement.gameObject);
            }
            Instance._minimapElements.Clear();
            
            var backgroundSize = Instance.minimapSize + 20;
            Instance.backgroundImage.rectTransform.sizeDelta = new Vector2(backgroundSize, backgroundSize);
            Instance.scale = (float)Instance.minimapSize / Mathf.Max(worldSize.x, worldSize.y);
            Instance.worldImage.rectTransform.sizeDelta = (Vector2)worldSize * Instance.scale;
        }

        public void RenderOnMinimap(string key, IRenderStrategy renderStrategy)
        {
            var spawned = renderStrategy.Render(out var minimapElement, key);
            if (!spawned) return;

            Instance._minimapElements.TryAdd(key, minimapElement);
        }

        public MinimapElement SpawnMinimapElement(Vector2 worldPos)
        {
            var prefab = GameManager.GetPrefab<MinimapElement>(PrefabNames.BaseMinimapElement);
            var spawnedElement = Instantiate(prefab, Instance.worldImage.rectTransform);
            var minimapPosition = worldPos * Instance.scale;
            spawnedElement.transform.SetParent(Instance.worldImage.rectTransform);
            spawnedElement.transform.localPosition = minimapPosition;
            return spawnedElement;
        }
    }
}