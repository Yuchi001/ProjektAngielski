using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class FrameCounter : MonoBehaviour
    {
        private TextMeshProUGUI frameText;
        
        private readonly float[] frameDeltaTimeArray = new float[100];
        private int lastFrameIndex = 0;

        private void Awake()
        {
            frameText = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            frameDeltaTimeArray[lastFrameIndex] = Time.unscaledDeltaTime;
            lastFrameIndex = (lastFrameIndex + 1) % frameDeltaTimeArray.Length;

            frameText.text = ((int)(frameDeltaTimeArray.Length / frameDeltaTimeArray.Sum())).ToString(CultureInfo.InvariantCulture);
        }
    }
}