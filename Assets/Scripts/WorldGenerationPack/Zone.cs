using MinimapPack;
using MinimapPack.Strategies;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace WorldGenerationPack
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Zone : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public bool ContainsEntity(Vector2 pos)
        {
            return spriteRenderer.bounds.Contains(pos);
        }

        public Vector2 GetRandomPos()
        {
            return spriteRenderer.GetRandomPoint();
        }

        public bool InRange(Vector2 pos, float range)
        {
            return ContainsEntity(pos) || Vector2.Distance(transform.position, pos) <= range;
        }

        public void Resize(float percentage)
        {
            var newScale = transform.localScale;
            newScale.x += percentage;
            newScale.y += percentage;
            
            transform.LeanScale(newScale, 0.3f).setEaseInBack().setEaseOutBack();
        }

        public void SetSize(float scale, bool anim)
        {
            WorldGeneratorManager.MinimapManager.RenderOnMinimap($"ZONE{GetInstanceID()}", new ZoneRenderStrategy(this));
            
            scale *= 2;
            var randomDivision = Random.Range(0.4f, 0.6f);
            var vec = new Vector3
            {
                x = scale * randomDivision,
                y = scale * (1 - randomDivision)
            };
            if (!anim)
            {
                transform.localScale = vec;
                return;
            }

            transform.LeanScale(vec, 0.3f).setEaseInBack().setEaseOutBack();
        }
    }
}