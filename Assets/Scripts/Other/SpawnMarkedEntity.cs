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

        private static readonly string SPAWN_TIMER_ID = "SPAWN_TIMER";

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
            SetTimer(SPAWN_TIMER_ID);
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

        public override void InvokeUpdate()
        {
            if (CheckTimer(SPAWN_TIMER_ID) < spawnTime) return;

            _invoker.SpawnRandomEntity(transform.position);
            
            MarkerManager.Instance.ReleasePoolObject(this);
            SetTimer(SPAWN_TIMER_ID);
        }
    }
}