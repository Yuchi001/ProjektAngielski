using PlayerPack;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeaponPack.SO;

namespace UI
{
    public class WeaponSlotUi : MonoBehaviour
    {
        [SerializeField] private Image weaponImage;
        [SerializeField] private TextMeshProUGUI weaponDescriptionField;

        private PlayerWeaponry PlayerWeaponry => PlayerManager.Instance.PlayerWeaponry;
        private SoWeapon _weapon;
        private GameObject _levelUpUiGameObject;

        public void Setup(SoWeapon weapon, GameObject levelUpUiGameObject)
        {
            _weapon = weapon;
            _levelUpUiGameObject = levelUpUiGameObject;
            weaponDescriptionField.text = PlayerWeaponry.GetWeaponDescription(weapon);
            weaponImage.sprite = weapon.WeaponSprite;
        }

        public void OnClick()
        {
            PlayerWeaponry.AddWeapon(_weapon);
            Destroy(_levelUpUiGameObject);
        }
    }
}