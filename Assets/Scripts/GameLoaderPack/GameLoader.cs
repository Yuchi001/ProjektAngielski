using System.Linq;
using Managers;
using MapPack;
using UnityEngine;

namespace GameLoaderPack
{
    public class GameLoader : MonoBehaviour
    {
        private static GameLoader Instance { get; set; }

        public static bool HasInstance() => Instance != null;
        
        // always this is an instance
        private void Awake()
        {
            if (!GameManager.HasInstance()) GameManager.LoadMenu();
            Instance = this;
        }

        public static bool LoadScene(MapManager.MissionData missionData)
        {
            var loadObjects = FindObjectsOfType<MonoBehaviour>().OfType<IMissionDependentInstance>();
            foreach (var obj in loadObjects)
            {
                obj.Init(missionData);
            }
            return true;
        }
    }
}