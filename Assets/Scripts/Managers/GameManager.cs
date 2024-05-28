using System;
using System.Collections.Generic;
using MainCameraPack;
using Managers.Enums;
using PlayerPack;
using PlayerPack.SO;
using UnityEngine;
using UnityEngine.Serialization;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
       #region Singleton
       
       public static GameManager Instance { get; private set; }

       private void Awake()
       {
           if (Instance != this && Instance != null) Destroy(gameObject);
           else Instance = this;

           Init();
       }
       
       #endregion

       [SerializeField] private GameObject deathUi;
       [SerializeField] private GameObject playerPrefab;
       [SerializeField] private MainCamera mainCamera;
       [SerializeField] private Transform worldCanvas;
       [SerializeField] private Transform mainCanvas;

       [SerializeField] private Transform menuCanvas;

       #region Prefabs

       [Header("Public prefabs")] 
       [SerializeField] private GameObject spawnEntityPrefab;
       public GameObject SpawnEntityPrefab => spawnEntityPrefab;

       #endregion

       public Transform WorldCanvas => worldCanvas;
       public Transform MainCanvas => mainCanvas;
       public PlayerManager CurrentPlayer { get; private set; }

       private void Init()
       {
           LeanTween.init(1000000, 1000000);
       }

       public void StartRun(SoCharacter pickedCharacter)
       {
           var playerObj = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
           CurrentPlayer = playerObj.GetComponent<PlayerManager>();
           
           foreach (Transform child in transform)
           {
               child.gameObject.SetActive(true);
           }
           
           menuCanvas.gameObject.SetActive(false);
           
           CurrentPlayer.Setup(pickedCharacter);
           
           mainCamera.Setup(playerObj);
           
           AudioManager.Instance.SetTheme(EThemeType.Main1);
       }

       public void OnPlayerDeath()
       {
           foreach (Transform child in transform)
           {
               child.gameObject.SetActive(false);
           }
           
           deathUi.SetActive(true);
       }
    }
}