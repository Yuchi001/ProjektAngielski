using UnityEngine;

namespace WeaponPack.Other
{
    public class ForceField : MonoBehaviour
    {
        private bool _ready = false;

        #region Setup methods

        public void SetReady()
        {
            _ready = true;
        }            

        #endregion
    }
}