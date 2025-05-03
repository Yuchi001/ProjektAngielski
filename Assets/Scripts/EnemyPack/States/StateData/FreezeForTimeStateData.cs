using UnityEngine;

namespace EnemyPack.States.StateData
{
    public class FreezeForTimeStateData : FreezeStateData
    {
        [SerializeField] private float freezeTime;

        public float FreezeTime => freezeTime;
    }
}