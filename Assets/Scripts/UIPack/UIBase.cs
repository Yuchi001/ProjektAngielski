using UnityEngine;

namespace UIPack
{
    public abstract class UIBase : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        public Animator Animator => animator;
        public bool Open => gameObject.activeSelf;

        public abstract void OnOpen();
        public abstract void OnClose();
    }
}