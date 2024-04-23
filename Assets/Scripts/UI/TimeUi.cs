using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class TimeUi : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timeText;

        private float _time = 0;

        private void Update()
        {
            _time += Time.deltaTime;

            timeText.text = TimeSpan.FromSeconds(_time).ToString(@"mm\:ss");
        }
    }
}