namespace EnchantmentPack.Enchantments
{
    public class PoisonShare : EnchantmentBase
    {
        public const float infectionRange = 0.3f;
        public override string GetDescriptionText()
        {
            return $"When poisoned enemy dies, all enemies in range of {infectionRange} are poisoned.";
        }
    }
}