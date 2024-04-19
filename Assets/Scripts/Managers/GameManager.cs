using System;
using PlayerPack;
using PlayerPack.SO;
using UnityEngine;

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

       private void Init()
       {
           var playerObj = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
           // todo: this shit is debug only replace with valid code later
           playerObj.GetComponent<PlayerManager>().Setup(debugCharacter);
       }
    }
}