using System;
using UnityEngine;
using UnityEngine.UI;

namespace MinimapPack
{
    public class MinimapElement : MonoBehaviour
    {
        [SerializeField] private Image mainImage;

        public Image MainImage => mainImage;

        protected virtual void Awake()
        {
            var size = MinimapManager.IconSize;
            mainImage.rectTransform.sizeDelta = new Vector2(size, size);
        }
    }
}