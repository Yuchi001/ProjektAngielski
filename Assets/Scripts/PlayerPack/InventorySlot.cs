using System;
using System.Collections.Generic;
using System.Linq;
using InventoryPack;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerPack
{
    public class InventorySlot : ItemSlot
    {
        [SerializeField] private Image frameImage;
        [SerializeField] private List<StateData> states;

        private Dictionary<EState, StateData> _stateDict;
        private EState _state = EState.PASSIVE;

        private void Awake()
        {
            _stateDict = states.ToDictionary(s => s.state, s => s);
        }

        
        public void SetSlotState(EState state)
        {
            if (!_stateDict.ContainsKey(state)) return;

            _state = state;
            var data = _stateDict[state];
            frameImage.color = data.frameColor;
        }

        [System.Serializable]
        public struct StateData
        {
            public EState state;
            public Color frameColor;
            public Color itemColor;
        }

        public enum EState
        {
            ACTIVE,
            PASSIVE
        }
    }
}