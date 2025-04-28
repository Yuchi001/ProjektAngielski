using System;
using AudioPack;
using Managers.Enums;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UIPack.Elements
{
    public class Button : NavigationElement, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler
    {
        [SerializeField] private UnityEvent onClick;
        [SerializeField] private Transform animationObject;

        public bool Active { get; private set; } = true;

        public virtual void SetActive(bool active)
        {
            Active = active;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            LeanTween.cancel(gameObject);
            animationObject.LeanScale(Vector2.one * 1.2f, 0.1f).setEaseInBack().setEaseInOutBack().setIgnoreTimeScale(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            LeanTween.cancel(gameObject);
            animationObject.LeanScale(Vector2.one, 0.1f).setEaseInBack().setEaseInOutBack().setIgnoreTimeScale(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            LeanTween.cancel(gameObject);
            animationObject.LeanScale(Vector2.one * 1.2f, 0.1f).setEaseInBack().setEaseInOutBack().setIgnoreTimeScale(true);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            LeanTween.cancel(gameObject);
            animationObject.LeanScale(Vector2.one * 0.8f, 0.1f).setEaseInBack().setEaseInOutBack().setIgnoreTimeScale(true);
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick();
        }

        private void OnDisable()
        {
            animationObject.transform.localScale = Vector2.one;
        }

        public override void OnFocus()
        {
            
        }

        public override void OnClick()
        {
            onClick?.Invoke();
            AudioManager.PlaySound(ESoundType.ButtonClick);
        }
    }
}