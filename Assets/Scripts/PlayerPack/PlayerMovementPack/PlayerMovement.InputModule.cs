using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace PlayerPack.PlayerMovementPack
{
    public partial class PlayerMovement
    {
        private Dictionary<KeyCode, bool> _buttonsActive;
        
        private Vector2 GetVelocity()
        {
            var movement = Vector2.zero;

            CheckKey(GameManager.UpBind, GameManager.DownBind);
            CheckKey(GameManager.LeftBind, GameManager.RightBind);

            movement.x = _buttonsActive[GameManager.RightBind] ? 1 : (_buttonsActive[GameManager.LeftBind] ? -1 : 0);
            movement.y = _buttonsActive[GameManager.UpBind] ? 1 : (_buttonsActive[GameManager.DownBind] ? -1 : 0);

            if(movement == Vector2.zero) return Vector2.zero;
            
            return movement * PickedCharacter.MovementSpeed;
        }

        public void ResetKeys()
        {
            foreach (var key in new List<KeyCode>(_buttonsActive.Keys))
            {
                _buttonsActive[key] = Input.GetKey(key);
            }
        }

        private void CheckKey(KeyCode main, KeyCode opposite)
        {
            if (Input.GetKeyDown(main))
            {
                _buttonsActive[main] = true;
                _buttonsActive[opposite] = false;
            }
            if (Input.GetKeyDown(opposite))
            {
                _buttonsActive[opposite] = true;
                _buttonsActive[main] = false;
            }

            if (Input.GetKeyUp(main))
            {
                if (Input.GetKey(opposite))
                {
                    _buttonsActive[opposite] = true;
                }
                _buttonsActive[main] = false;
            }

            if (Input.GetKeyUp(opposite))
            {
                if (Input.GetKey(main))
                {
                    _buttonsActive[main] = true;
                }
                _buttonsActive[opposite] = false;
            }
        }
    }
}