using System;
using PlayerPack;
using UnityEngine;

namespace Managers
{
    //TODO: uzupelnic klase
    public class GameUiManager : MonoBehaviour
    {
        [SerializeField] private Transform gameCanvas;
        [SerializeField] private Transform deathCanvas;
        [SerializeField] private Transform menuCanvas;
        [SerializeField] private Transform worldCanvas;
        
        public Transform WorldCanvas => worldCanvas;
        public Transform GameCanvas => gameCanvas;
        
        public static GameUiManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
            
            //menuCanvas.gameObject.SetActive(true);

            PlayerManager.OnPlayerDeath += OnPlayerDeath;
        }

        private void OnDisable()
        {
            PlayerManager.OnPlayerDeath -= OnPlayerDeath;
        }

        private void OnPlayerDeath()
        {
            /*gameCanvas.gameObject.SetActive(false);
            menuCanvas.gameObject.SetActive(false);
            deathCanvas.gameObject.SetActive(true);*/
        }

        public void BeginPlay()
        {
            /*worldCanvas.gameObject.SetActive(true);
            gameCanvas.gameObject.SetActive(true);
            menuCanvas.gameObject.SetActive(false);*/
        }
    }
}