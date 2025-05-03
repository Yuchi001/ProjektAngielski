using PlayerPack.Decorators;

namespace PlayerPack.Interface
{
    public interface IDamageModifier
    {
        public bool QueueAsLast { get; protected set; }
        public void ModifyDamageContext(DamageContext context);
    }
}