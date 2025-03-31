using System;
using PlayerPack.SO;
using UnityEngine;

namespace Managers
{
    public class MissionManager : MonoBehaviour
    {
        private static MissionManager Instance { get; set; }

        private SoCharacter _pickedCharacter;

        private void Awake()
        {
            if (Instance != null && Instance != this) Destroy(gameObject);
            else Instance = this;
        }

        public void PickCharacter(SoCharacter character)
        {
            // TODO: Pick character logic
            _pickedCharacter = character;
        }

        public void StartMission()
        {
            // TODO: start mission logic
            GameManager.StartRun(_pickedCharacter);
        }
    }
}