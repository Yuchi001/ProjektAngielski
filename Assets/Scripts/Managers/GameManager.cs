using System;
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

       [SerializeField] private SoCharacter debugCharacter;
       [SerializeField] private GameObject playerPrefab;
       [SerializeField] private MainCamera mainCamera;
       [SerializeField] private Transform worldCanvas;
       [SerializeField] private Transform mainCanvas;

       public Transform WorldCanvas => worldCanvas;
       public Transform MainCanvas => mainCanvas;
       public PlayerManager CurrentPlayer { get; private set; }

       private void Init()
       {
           LeanTween.init(100, 100);
           // todo: this shit is debug only replace with valid code later
           StartRun(debugCharacter);
       }

       public void StartRun(SoCharacter pickedCharacter)
       {
           var playerObj = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
           CurrentPlayer = playerObj.GetComponent<PlayerManager>();
           CurrentPlayer.Setup(pickedCharacter);
           
           mainCamera.Setup(playerObj);
       }
    }
}