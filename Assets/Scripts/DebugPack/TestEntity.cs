using Other;

namespace DebugPack
{
    public class TestEntity : CanBeDamaged
    {
        public override int CurrentHealth { get; } = 99999;
        public override int MaxHealth { get; } = 99999;

        protected override void LazyUpdate(float lazyDeltaTime)
        {
            // IGNORE
        }
    }
}