using PlayerPack;
using UnityEngine;
using UnityEngine.UI;
using WeaponPack;

namespace UI
{
    public class DisplayWeaponsUi : MonoBehaviour
    {
        [SerializeField] private RectTransform slotContainer;
        [SerializeField] private GameObject displaySlotPrefab;
        private void Awake()
        {
            PlayerWeaponry.OnWeaponAdd += SpawnDisplaySlot;
        }

        private void OnDisable()
        {
            PlayerWeaponry.OnWeaponAdd -= SpawnDisplaySlot;
        }

        private void SpawnDisplaySlot(WeaponLogicBase weaponLogicBase)
        {
            var displayUI = Instantiate(displaySlotPrefab, slotContainer.position, Quaternion.identity, slotContainer);
            var displayUiScript = displayUI.GetComponent<DisplayWeaponSlotUi>();
            displayUiScript.Setup(weaponLogicBase);
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(slotContainer);
        }
    }
}