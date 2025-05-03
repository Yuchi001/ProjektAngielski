using System;
using System.Collections.Generic;
using System.Linq;
using AudioPack;
using Managers.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIPack.Elements
{
    public class Button : NavigationElement, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler
    {
        [SerializeField] private UnityEvent onClick;
        [SerializeField] private Color enabledColor = Color.white;
        [SerializeField] private Color disabledColor = Color.grey;
        [SerializeField] private Graphic animationObject;
        
        public bool Active { get; private set; } = true;
        
        protected TextMeshProUGUI _textField;
        protected List<Graphic> _graphicElements;

        private void Awake()
        {
            _textField = animationObject.GetComponentInChildren<TextMeshProUGUI>();
            _graphicElements = animationObject.GetComponentsInChildren<Graphic>().ToList();
        }

        public virtual void EnableButton(bool active)
        {
            Active = active;
            var color = active ? enabledColor : disabledColor;
            _graphicElements.ForEach(i => i.color = color);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!Active) return;
            LeanTween.cancel(gameObject);
            animationObject.transform.LeanScale(Vector2.one * 1.2f, 0.1f).setEaseInBack().setEaseInOutBack().setIgnoreTimeScale(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!Active) return;
            LeanTween.cancel(gameObject);
            animationObject.transform.LeanScale(Vector2.one, 0.1f).setEaseInBack().setEaseInOutBack().setIgnoreTimeScale(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!Active) return;
            LeanTween.cancel(gameObject);
            animationObject.transform.LeanScale(Vector2.one * 1.2f, 0.1f).setEaseInBack().setEaseInOutBack().setIgnoreTimeScale(true);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!Active) return;
            LeanTween.cancel(gameObject);
            animationObject.transform.LeanScale(Vector2.one * 0.8f, 0.1f).setEaseInBack().setEaseInOutBack().setIgnoreTimeScale(true);
        }

        private void Update()
        {
            if (Active) return;
            animationObject.transform.localScale = Vector3.one; 
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
            if (!Active) return;
            
            onClick?.Invoke();
            AudioManager.PlaySound(ESoundType.ButtonClick);
        }
    }
}