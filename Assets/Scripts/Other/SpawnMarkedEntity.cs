using MarkerPackage;
using PoolPack;
using UnityEngine;
using WorldGenerationPack;

namespace Other
{
    public class SpawnMarkedEntity : PoolObject
    {
        [SerializeField] private float spawnTime;

        private SpriteRenderer _spriteRenderer;

        private IUseMarker _invoker;

        private float _timer;

        #region Setup methods

        public override void OnCreate(PoolManager poolManager)
        {
            base.OnCreate(poolManager);
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetReady(IUseMarker invoker, MarkerManager markerManager)
        {
            _invoker = invoker;

            _spriteRenderer.color = invoker.GetMarkerColor();
            _timer = 0;
            var randomPosition = ZoneGeneratorManager.GetRandomPos();
            if (!randomPosition.HasValue)
            {
                markerManager.ReleasePoolObject(this);
                return;
            }
            
            transform.position = randomPosition.Value;
            OnGet(null);
        }

        #endregion

        public void Update()
        {
            _timer += Time.deltaTime;
            if (_timer < spawnTime) return;

            _invoker.SpawnRandomEntity(transform.position);
            
            MarkerManager.Instance.ReleasePoolObject(this);
        }
    }
}