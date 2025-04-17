using System.Collections.Generic;
using Managers;
using Managers.Other;
using UIPack;
using UIPack.CloseStrategies;
using UIPack.OpenStrategies;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerPack.PlayerItemPack
{
    public class PlayerEqManager : MonoBehaviour
    {
        private IOpenStrategy _openStrategy;
        private ICloseStrategy _closeStrategy;

        private const string PLAYER_EQ_KEY = "PLAYER_EQ_KEY";

        private PlayerEqUI _current;

        private void Awake()
        {
            var pref = GameManager.GetPrefab<PlayerEqUI>(PrefabNames.PlayerEqUI);
            _openStrategy = new SingletonOpenStrategy<PlayerEqUI>(pref);
            _closeStrategy = new DefaultCloseStrategy();
        }

        private readonly List<PlayerManager.State> _validStates = new()
        {
            PlayerManager.State.IN_MAP,
            PlayerManager.State.ON_MISSION
        };

        public void OnEqToggle(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started || _validStates.Contains(PlayerManager.CurrentState)) return;

            if (_current == null || !_current.Open)
            {
                _current = UIManager.OpenUI<PlayerEqUI>(PLAYER_EQ_KEY, _openStrategy, _closeStrategy);
                return;
            }

            UIManager.CloseUI(PLAYER_EQ_KEY);
        }
    }
}