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

        public void OnPointerEnter(PointerEventData eventData)
        {
            LeanTween.cancel(gameObject);
            transform.LeanScale(Vector2.one * 1.2f, 0.1f).setEaseInBack().setEaseInOutBack();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            LeanTween.cancel(gameObject);
            transform.LeanScale(Vector2.one, 0.1f).setEaseInBack().setEaseInOutBack();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            LeanTween.cancel(gameObject);
            transform.LeanScale(Vector2.one, 0.1f).setEaseInBack().setEaseInOutBack();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            LeanTween.cancel(gameObject);
            transform.LeanScale(Vector2.one * 0.8f, 0.1f).setEaseInBack().setEaseInOutBack();
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick();
        }

        public override void OnFocus()
        {
            throw new System.NotImplementedException();
        }

        public override void OnClick()
        {
            onClick?.Invoke();
            AudioManager.PlaySound(ESoundType.ButtonClick);
        }
    }
}