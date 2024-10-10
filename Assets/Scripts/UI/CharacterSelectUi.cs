using System.Collections.Generic;
using System.Linq;
using Managers;
using Managers.Enums;
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
        [SerializeField] private Image pickedCharacterWeaponImage;

        [SerializeField] private Image rightArrow;
        [SerializeField] private Image leftArrow;

        [SerializeField] private Sprite _emptySprite;
        [SerializeField] private List<StatDisplay> _statDisplays = new();
        
        private List<SoCharacter> _availableCharacters;

        public delegate void PickCharacterDelegate(string characterName);
        public static event PickCharacterDelegate OnPickCharacter;

        private int _currentIndex = 0;
        
        private void Start()
        {
            AudioManager.Instance.SetTheme(EThemeType.Menu1);
            
            var allCharacters = new List<SoCharacter>(Resources.LoadAll<SoCharacter>("Characters").Select(Instantiate).ToList());
            // todo: show only available characters
            _availableCharacters = allCharacters; // .Where(c => c.CharacterName != "Debug").ToList();
            
            leftArrow.color = Color.clear;
            PickCharacter();
        }

        private void Update()
        {
            if (Input.GetKeyDown(GameManager.AcceptBind))
            {
                AudioManager.Instance.PlaySound(ESoundType.ButtonClick);
                GameManager.Instance.StartRun(_availableCharacters[_currentIndex]);
            }

            if (_availableCharacters.Count <= 1) return;

            if (Input.GetKeyDown(GameManager.LeftBind))
            {
                AudioManager.Instance.PlaySound(ESoundType.ButtonClick);
                _currentIndex--;
                rightArrow.color = Color.white;
                if (_currentIndex < 1)
                {
                    _currentIndex = 0;
                    leftArrow.color = Color.clear;
                }
            }
            
            if (Input.GetKeyDown(GameManager.RightBind))
            {
                AudioManager.Instance.PlaySound(ESoundType.ButtonClick);
                _currentIndex++;
                leftArrow.color = Color.white;
                if (_currentIndex + 1 >= _availableCharacters.Count)
                {
                    _currentIndex = _availableCharacters.Count - 1;
                    rightArrow.color = Color.clear;
                }
            }
            
            PickCharacter();
        }

        private void PickCharacter()
        {
            var pickedCharacter = _availableCharacters[_currentIndex];
            pickedCharacterImage.sprite = pickedCharacter.CharacterSprite;
            pickedCharacterName.text = pickedCharacter.CharacterName;
            pickedCharacterWeaponImage.sprite = pickedCharacter.StartingWeapon.WeaponSprite;

            var values = new List<float>()
            {
                pickedCharacter.MaxHp,
                pickedCharacter.MovementSpeed,
                pickedCharacter.MaxWeaponsInEq,
                pickedCharacter.MaxDashStacks,
            };
            
            for (var i = 0; i < _statDisplays.Count; i++)
            {
                var statDisplay = _statDisplays[i];
                var displayValue = statDisplay.GetActiveImagesCount(values[i], i == 1 ? 2f : 0);
                var images = statDisplay.GetImages();
                for (var j = 0; j < images.Count; j++)
                {
                    images[j].sprite = j < displayValue ? statDisplay.ActiveSprite : _emptySprite;
                }
            }
            
            OnPickCharacter?.Invoke(pickedCharacter.CharacterName);
        }
    }

    [System.Serializable]
    public class StatDisplay
    {
        [SerializeField] private Sprite activeSprite;
        [SerializeField] private Transform container;
        [SerializeField] private float step;

        public Sprite ActiveSprite => activeSprite;

        public List<Image> GetImages()
        {
            var list = new List<Image>();
            foreach (Transform child in container)
            {
                if(!child.TryGetComponent(out Image img)) continue;
                list.Add(img);
            }

            return list;
        }

        public int GetActiveImagesCount(float value, float minValue = 0)
        {
            return (int)((value - minValue) / step);
        }
    }
}