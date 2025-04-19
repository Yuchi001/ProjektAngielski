using PlayerPack.Decorators;

namespace PlayerPack.Interface
{
    public interface IHealModifier
    {
        public bool QueueAsLast { get; protected set; }
        public void ModifyHealContext(HealContext healContext);
    }
}