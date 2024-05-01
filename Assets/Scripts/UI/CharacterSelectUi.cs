using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using PlayerPack.SO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CharacterSelectUi : MonoBehaviour
    {
        [SerializeField] private Image pickedCharacterImage;
        [SerializeField] private TextMeshProUGUI pickedCharacterName;
        [SerializeField] private TextMeshProUGUI pickedCharacterWeaponDescription;
        [SerializeField] private TextMeshProUGUI pickedCharacterWeaponName;
        [SerializeField] private TextMeshProUGUI pickedCharacterWeaponCount;
        [SerializeField] private TextMeshProUGUI pickedCharacterMs;
        [SerializeField] private TextMeshProUGUI pickedCharacterHp;
        [SerializeField] private Image pickedCharacterWeaponImage;
        [SerializeField] private RectTransform slotContainer;
        [SerializeField] private GameObject characterSlotPrefab;

        private int RowLength
        {
            get
            {
                var availableCount = _availableCharacters.Count;
                var rowLength = slotContainer.GetComponent<GridLayoutGroup>().constraintCount;
                return availableCount >= rowLength ? rowLength : availableCount;
            }
        }

        private List<SoCharacter> _availableCharacters;

        public delegate void PickCharacterDelegate(string characterName);
        public static event PickCharacterDelegate OnPickCharacter;

        private int _currentIndex = 0;
        
        private void Awake()
        {
            var allCharacters = Resources.LoadAll<SoCharacter>("Characters").Select(Instantiate).ToList();
            // todo: show only available characters
            _availableCharacters = allCharacters.Where(c => c.CharacterName != "Debug").ToList();
            
            foreach (var character in allCharacters)
            {
                var characterSlotInstance = Instantiate(characterSlotPrefab, slotContainer, false);
                var characterSlotScript = characterSlotInstance.GetComponent<CharacterSlotUi>();
                if(_availableCharacters.Contains(character)) characterSlotScript.Setup(character);
                else characterSlotScript.Setup();
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(slotContainer);
            
            PickCharacter();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameManager.Instance.StartRun(_availableCharacters[_currentIndex]);
            }

            // just in case _currentIndex will be out of range, this code is shit ikr?
            var current = _currentIndex;
            
            if (Input.GetKeyDown(KeyCode.A))
            {
                _currentIndex--;
                if (_currentIndex < 0) _currentIndex = _availableCharacters.Count - 1;
            }
            
            if (Input.GetKeyDown(KeyCode.D))
            {
                _currentIndex++;
                if (_currentIndex >= _availableCharacters.Count) _currentIndex = 0;
            }
            
            if (Input.GetKeyDown(KeyCode.S))
            {
                _currentIndex += RowLength;
                if (_currentIndex >= _availableCharacters.Count) _currentIndex = current - RowLength;
                if (_currentIndex < 0) _currentIndex = current;
            }
            
            if (Input.GetKeyDown(KeyCode.W))
            {
                _currentIndex -= RowLength;
                if (_currentIndex < 0) _currentIndex = current + RowLength;
                if (_currentIndex >= _availableCharacters.Count) _currentIndex = current;
            }
            
            PickCharacter();
        }

        private void PickCharacter()
        {
            var pickedCharacter = _availableCharacters[_currentIndex];
            pickedCharacterImage.sprite = pickedCharacter.CharacterSprite;
            pickedCharacterName.text = pickedCharacter.CharacterName;
            pickedCharacterWeaponDescription.text = pickedCharacter.StartingWeapon.WeaponDescription;
            pickedCharacterWeaponName.text = $"Default weapon: {pickedCharacter.StartingWeapon.WeaponName}";
            pickedCharacterWeaponImage.sprite = pickedCharacter.StartingWeapon.WeaponSprite;
            pickedCharacterHp.text = $"Hp: {pickedCharacter.MaxHp.ToString()}";
            pickedCharacterMs.text = $"Ms: {pickedCharacter.MovementSpeed.ToString()}";
            pickedCharacterWeaponCount.text = $"Wc: {pickedCharacter.MaxWeaponsInEq.ToString()}";
            
            OnPickCharacter?.Invoke(pickedCharacter.CharacterName);
        }
    }
}