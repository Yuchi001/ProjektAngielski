using PlayerPack;

namespace EnemyPack.States.StateData
{
    public class MeleeAttackStateData : AttackStateData
    {
        public void DamagePlayer()
        {
            PlayerManager.PlayerHealth.GetDamaged(damage);
        }
    }
}