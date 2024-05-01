using System;
using PlayerPack.SO;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CharacterSlotUi : MonoBehaviour
    {
        [SerializeField] private Sprite emptyCharacterSprite;
        [SerializeField] private Image characterImage;

        private SoCharacter _character = null;

        private void Awake()
        {
            CharacterSelectUi.OnPickCharacter += OnPickCharacter;
        }

        private void OnDisable()
        {
            CharacterSelectUi.OnPickCharacter -= OnPickCharacter;
        }

        public void Setup(SoCharacter character)
        {
            _character = character;
            characterImage.sprite = character.CharacterSprite;
        }
        
        public void Setup()
        {
            characterImage.sprite = emptyCharacterSprite;
        }

        private void OnPickCharacter(string characterName)
        {
            if (_character == null) return;
            
            var scale = _character.CharacterName == characterName ? 1.1f : 1;
            transform.localScale = new Vector3(scale, scale, scale);
        }
    }
}