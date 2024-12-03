using ItemPack.SO;
using ItemPack.WeaponPack.WeaponsLogic;
using PlayerPack;
using UnityEngine;

namespace ItemPack.WeaponPack.SideClasses
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

        private void SetScale(SoItem item)
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