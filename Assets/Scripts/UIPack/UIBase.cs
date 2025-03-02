using System.Collections;
using UnityEngine;

namespace UIPack
{
    public abstract class UIBase : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private float closeTime = 0.3f;
        [SerializeField] private bool useAnim = false;
        public bool Open => gameObject != null && gameObject.activeSelf;
        protected string Key { get; private set; }
        
        public virtual void OnOpen(string key)
        {
            gameObject.SetActive(true);
            if (useAnim) animator.SetTrigger("OPEN");
            Key = key;
        }

        public virtual void OnClose()
        {
            if (useAnim) animator.SetTrigger("CLOSE");
            StartCoroutine(Deactivate());
        }

        private IEnumerator Deactivate()
        {
            yield return new WaitForSeconds(useAnim ? closeTime : 0);
            gameObject.SetActive(false);
        }
    }
}