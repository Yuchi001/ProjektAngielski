using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace UIPack
{
    public abstract class UIBase : MonoBehaviour
    {
        [SerializeField] private float animTime = 0.3f;
        [SerializeField] private bool useAnim = false;
        public bool Open => gameObject != null && gameObject.activeSelf;
        protected string Key { get; private set; }
        
        public virtual void OnOpen(string key)
        {
            gameObject.SetActive(true);
            if (useAnim) transform.LeanScale(Vector3.one, animTime).setEaseInBack().setEaseOutBack();
            Key = key;
        }

        public virtual void OnClose()
        {
            if (useAnim) transform.LeanScale(Vector3.zero, animTime).setEaseInBack().setEaseOutBack();
            StartCoroutine(Deactivate());
        }

        private IEnumerator Deactivate()
        {
            yield return new WaitForSeconds(useAnim ? animTime : 0);
            gameObject.SetActive(false);
        }
    }
}