using UnityEngine;
using UnityEngine.Serialization;

namespace EnemyPack.States.StateData
{
    public class FreezeStateData : StateDataBase
    {
        [SerializeField] private bool useFreezeSprite;
        [SerializeField] private bool invincible;
        [SerializeField] private Sprite freezeSprite;

        public bool UseFreezeSprite => useFreezeSprite;
        public bool Invincible => invincible;
        public Sprite FreezeSprite => freezeSprite;
    }
}