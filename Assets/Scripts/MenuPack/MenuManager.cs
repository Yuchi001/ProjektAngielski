using System;
using System.Collections;
using MainCameraPack;
using Managers;
using PlayerPack;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MenuPack
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private Transform cameraPose;
        [SerializeField] private float cameraSize;
        
        private void Awake()
        {
            if (GameManager.HasInstance()) return;
            
            GameManager.LoadGameScene(LoadSceneMode.Additive);
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(MainCamera.HasInstance);
            
            MainCamera.SetSize(cameraSize);
            MainCamera.SetFollow(cameraPose);
        }

        public void OnStart()
        {
            MainCamera.InOutAnim(0.3f, GameManager.GoToTavern, () => {});
        }

        public void OnOptions()
        {
            // TODO: options
        }

        public void OnExit()
        {
            Application.Quit();
        }
    }
}