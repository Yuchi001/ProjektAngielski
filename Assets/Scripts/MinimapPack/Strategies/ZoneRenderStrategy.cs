using System;
using Unity.VisualScripting;
using UnityEngine;
using WorldGenerationPack;

namespace MinimapPack.Strategies
{
    public class ZoneRenderStrategy : IRenderStrategy
    {
        private Zone _zone;
        private MinimapElement _minimapElement;
        
        public ZoneRenderStrategy(Zone zone)
        {
            _zone = zone;
        }
        
        public bool Render(out MinimapElement minimapElement, string key)
        {
            _minimapElement = minimapElement = WorldGeneratorManager.MinimapManager.SpawnMinimapElement(_zone.transform.position);
            _minimapElement.AddComponent<MinimapZoneScaleTracker>().Setup(this, _zone);
            var color = Color.white;
            color.a = 0.3f;
            _minimapElement.MainImage.color = color;
            
            ReRender();
            return true;
        }

        public void ReRender()
        {
            _minimapElement.MainImage.rectTransform.sizeDelta = _zone.transform.localScale * MinimapManager.MinimapScale;
        }

        private class MinimapZoneScaleTracker : MonoBehaviour
        {
            private ZoneRenderStrategy _renderer;
            private Zone _zone;
            private Vector2 _lastScale;
            
            public void Setup(ZoneRenderStrategy rendererStrategy, Zone zone)
            {
                _renderer = rendererStrategy;
                _zone = zone;
                _lastScale = zone.transform.localScale;
            }
            
            private void Update()
            {
                if (_lastScale.magnitude >= _zone.transform.localScale.magnitude) return;

                _lastScale = _zone.transform.localScale;
                _renderer.ReRender();
            }
        }
    }
}