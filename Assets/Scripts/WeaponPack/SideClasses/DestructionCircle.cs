using PlayerPack;
using UnityEngine;
using WeaponPack.SO;
using WeaponPack.WeaponsLogic;

namespace WeaponPack.SideClasses
{
    public class DestructionCircle : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private float scaling = 8.1f;
        [SerializeField] private float animSpeed = 1;

        private BookOfDestructionLogic _bookOfDestructionLogic;

        public void Setup(BookOfDestructionLogic bookOfDestructionLogic)
        {
            _bookOfDestructionLogic = bookOfDestructionLogic;
            SetScale(null);
        }
        
        private void Awake()
        {
            PlayerWeaponry.OnWeaponLevelUp += SetScale;
        }

        private void OnDisable()
        {
            PlayerWeaponry.OnWeaponLevelUp -= SetScale;
        }

        private void SetScale(SoWeapon weapon)
        {
            var scale = _bookOfDestructionLogic.GetRange() / scaling;
            transform.localScale = Vector2.one * scale;
        }

        private void Update()
        {
            if (PlayerManager.Instance == null) return;
            transform.position = PlayerManager.Instance.transform.position;
            animator.speed = (1 - _bookOfDestructionLogic.TimerScaled) * animSpeed;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, scaling);
        }
    }
}