using ExpPackage;
using Managers;
using Managers.Enums;
using PlayerPack;
using UnityEngine;

namespace EnchantmentPack.Enchantments.SideClasses
{
    public class HealingOrb : MonoBehaviour
    {
        [SerializeField] private float range;
        [SerializeField] private float animTime;
        private static Vector3 PlayerPos => GameManager.Instance.CurrentPlayer.transform.position;
        
        private int healAmount = 0;

        private float _timer = 0;

        private ExpGem.EPickableState _pickableState = ExpGem.EPickableState.Default;
        private Vector2 startPosition;
        
        public void Setup(int amount)
        {
            healAmount = amount;
            startPosition = transform.position;
        }

        private void Update()
        {
            if (PlayerManager.Instance == null) return;

            switch (_pickableState)
            {
                case ExpGem.EPickableState.Default:
                    if (Vector2.Distance(transform.position, PlayerPos) > range) return;

                    _pickableState = ExpGem.EPickableState.PickingUpPhase;
                    return;
                case ExpGem.EPickableState.PickingUpPhase:
                    _timer += Time.deltaTime;
                    var remainingTime = Mathf.Clamp01(_timer / animTime);

                    transform.position = Vector3.Lerp(startPosition, PlayerPos, remainingTime);
                    
                    if (Vector2.Distance(transform.position, PlayerPos) > 0.1f) return;
                    
                    _pickableState = ExpGem.EPickableState.PickedUp;
                    
                    var playerExp = PlayerManager.Instance.PlayerHealth;
                    playerExp.Heal(healAmount, ESoundType.HealOrb);
                    Destroy(gameObject);
                    return;
                case ExpGem.EPickableState.PickedUp: return;
                default: return;
            }
        }
    }
}