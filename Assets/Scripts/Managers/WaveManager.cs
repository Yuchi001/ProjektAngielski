using System.Collections.Generic;
using Managers.Base;
using Managers.Enums;
using UnityEngine;

namespace Managers
{
    public class WaveManager : MonoBehaviour
    {
        [SerializeField] private List<SpawnerBase> spawners = new();

        public void BeginSpawn()
        {
            spawners.ForEach(s => s.SetState(ESpawnerState.Spawn));
        }
    }
}