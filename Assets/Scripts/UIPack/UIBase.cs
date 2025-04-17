using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace UIPack
{
    public abstract class UIBase : MonoBehaviour
    {
        [SerializeField] protected float animTime = 0.3f;
        [SerializeField] protected bool useAnim = false;
        public bool Open => gameObject != null && gameObject.activeSelf;
        protected string Key { get; private set; }
        
        public virtual void OnOpen(string key)
        {
            gameObject.SetActive(true);
            if (useAnim) transform.LeanScale(Vector3.one, animTime).setEaseInBack().setEaseOutBack().setIgnoreTimeScale(true);
            Key = key;
        }

        public virtual void OnClose()
        {
            if (useAnim) transform.LeanScale(Vector3.zero, animTime).setEaseInBack().setIgnoreTimeScale(true);
            StartCoroutine(Deactivate());
        }

        protected virtual void OnDeactivate()
        {
            gameObject.SetActive(false);
        }

        private IEnumerator Deactivate()
        {
            yield return new WaitForSecondsRealtime(animTime);
            
            OnDeactivate();
        }
    }
}