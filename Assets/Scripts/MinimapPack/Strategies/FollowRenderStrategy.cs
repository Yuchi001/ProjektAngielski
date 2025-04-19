using System;
using Unity.VisualScripting;
using UnityEngine;
using WorldGenerationPack;

namespace MinimapPack.Strategies
{
    public class FollowRenderStrategy : IRenderStrategy
    {
        private readonly Transform _followObject;
        private readonly Sprite _icon;
        private MinimapElement _minimapElement;
        
        public FollowRenderStrategy(Transform followObject, Sprite icon)
        {
            _followObject = followObject;
            _icon = icon;
        }
        
        public bool Render(out MinimapElement minimapElement, string key)
        {
            _minimapElement = minimapElement = WorldGeneratorManager.MinimapManager.SpawnMinimapElement(_followObject.transform.position);
            _minimapElement.AddComponent<FollowRenderMono>().Setup(_followObject, this);
            _minimapElement.MainImage.sprite = _icon;
            ReRender();
            return true;
        }

        private void ReRender()
        {
            _minimapElement.transform.localPosition = _followObject.transform.position * MinimapManager.MinimapScale;
        }

        private class FollowRenderMono : MonoBehaviour
        {
            private Transform _follow;
            private FollowRenderStrategy _renderStrategy;
            
            public void Setup(Transform followObj, FollowRenderStrategy renderStrategy)
            {
                _follow = followObj;
                _renderStrategy = renderStrategy;
            }
            
            private void Update()
            {
                if (_follow == null) return;
                _renderStrategy.ReRender();
            }
        }
    }
}