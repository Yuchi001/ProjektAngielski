using Other;

namespace PlayerPack.Decorators
{
    public class HealContext
    {
        public int Value { get; private set; }
        public CanBeDamaged CanBeDamaged { get; private set; }

        public HealContext(int value, CanBeDamaged canBeDamaged)
        {
            Value = value;
            CanBeDamaged = canBeDamaged;
        }

        public void ModifyValue(int value) => Value = value;
    }
}