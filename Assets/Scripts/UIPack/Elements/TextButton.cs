using TMPro;
using UnityEngine;

namespace UIPack.Elements
{
    public class TextButton : Button
    {
        [SerializeField] private TextMeshProUGUI textField;
        [SerializeField] private Color enabledColor;
        [SerializeField] private Color disabledColor;
        
        public override void SetActive(bool active)
        {
            base.SetActive(active);

            textField.color = active ? enabledColor : disabledColor;
        }

        public void SetText(string text)
        {
            textField.text = text;
        }
    }
}