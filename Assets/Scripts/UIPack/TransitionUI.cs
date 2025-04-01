using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIPack
{
    public class TransitionUI : UIBase
    {
        [SerializeField] private TextMeshProUGUI progressField;
        [SerializeField] private Image progressBar;
        
        public void SetLoadingProgress(float progress)
        {
            progressField.text = $"{progress}%";
            progressBar.fillAmount = progress;
        }
    }
}