using UnityEngine;

namespace EnemyPack.States.StateData
{
    public class FreezeStateData : StateDataBase
    {
        [SerializeField] private bool useFreezeSprite;
        [SerializeField] private Sprite freezeSprite;

        public bool UseFreezeSprite => useFreezeSprite;
        public Sprite FreezeSprite => freezeSprite;
    }
}