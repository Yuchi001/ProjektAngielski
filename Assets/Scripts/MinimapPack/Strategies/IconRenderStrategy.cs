using UnityEngine;
using WorldGenerationPack;

namespace MinimapPack.Strategies
{
    public class IconRenderStrategy : IRenderStrategy
    {
        private readonly Sprite _icon;
        private readonly Vector2 _position;
        
        public IconRenderStrategy(Sprite icon, Vector2 position)
        {
            _icon = icon;
            _position = position;
        }
        
        public bool Render(out MinimapElement minimapElement, string key)
        {
            minimapElement = MinimapManager.SpawnMinimapElement(_position);
            minimapElement.MainImage.sprite = _icon;
            return true;
        }
    }
}