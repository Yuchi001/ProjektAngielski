using System;
using System.Collections.Generic;
using System.Linq;
using AudioPack;
using InventoryPack;
using ItemPack.SO;
using Managers.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PlayerPack
{
    public class InventorySlot : ItemSlot
    {
        [SerializeField] private Image frameImage;
        [SerializeField] private List<StateData> states;

        public IEnumerable<StateData> States
        {
            get
            {
                foreach (EState state in Enum.GetValues(typeof(EState)))
                {
                    if (states.FirstOrDefault(s => s.state == state) != default) continue;
                    states.Add(new StateData
                    {
                        state = state
                    });
                }

                return states;
            }
        }

        private Dictionary<EState, StateData> _stateDict;
        private EState _state = EState.PASSIVE;

        private void Awake()
        {
            _stateDict = States.ToDictionary(s => s.state, s => s);
        }

        protected override void OnDragStart(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                PlayerManager.PlayerItemManager.ScrapItem(Index);
                return;
            }
            
            base.OnDragStart(eventData);
        }

        public void SetSlotState(EState state)
        {
            if (!_stateDict.ContainsKey(state)) return;

            _state = state;
            var data = _stateDict[state];
            frameImage.color = data.frameColor;
            defaultItemColor = data.itemColor;
        }

        public void SetStateData(List<StateData> stateData)
        {
            states = new List<StateData>(stateData);
        }

        [System.Serializable]
        public class StateData
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