using UnityEngine;

namespace UIPack.Elements
{
    [System.Serializable]
    public abstract class NavigationElement : MonoBehaviour
    {
        public abstract void OnFocus();

        public abstract void OnClick();
    }
}