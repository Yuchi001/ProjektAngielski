using System;
using System.Collections.Generic;
using Managers;
using Managers.Enums;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace UI
{
    public class DeathUi : MonoBehaviour
    {
        [SerializeField] private List<ButtonInfo> buttons = new();

        private int _currentIndex = 0;

        private void Awake()
        {
            for (int i = 0; i <  buttons.Count; i++)
            {
                var button = buttons[i].button;
                var scale = i == _currentIndex ? 1.1f : 1;
                button.transform.localScale = new Vector3(scale, scale, scale);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                AudioManager.Instance.PlaySound(ESoundType.ButtonClick);
                _currentIndex--;
                if (_currentIndex < 0) _currentIndex = buttons.Count - 1;
                
                UpdateButtonScale();
            }
            
            if (Input.GetKeyDown(KeyCode.S))
            {
                AudioManager.Instance.PlaySound(ESoundType.ButtonClick);
                _currentIndex++;
                if (_currentIndex >= buttons.Count) _currentIndex = 0;
                
                UpdateButtonScale();
            }

            if (!Input.GetKeyDown(KeyCode.Space)) return;

            buttons[_currentIndex].onClickEvent?.Invoke();
        }

        private void UpdateButtonScale()
        {
            for (var i = 0; i <  buttons.Count; i++)
            {
                var button = buttons[i].button;
                var scale = i == _currentIndex ? 1.1f : 1;
                button.transform.localScale = new Vector3(scale, scale, scale);
            }
        }

        public void OnQuit()
        {
            Application.Quit();
        }

        public void OnRestart()
        {
            // todo: setup scene loader
            SceneManager.LoadScene(0);
        }
    }

    [System.Serializable]
    public class ButtonInfo
    {
        public GameObject button;
        public UnityEvent onClickEvent;
    }
}