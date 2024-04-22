using PlayerPack;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LevelUpUi : MonoBehaviour
    {
        [SerializeField] private GameObject weaponSlotPrefab;
        [SerializeField] private RectTransform weaponContainer;
        [SerializeField] private int randomWeaponsCount = 3;

        public void Setup()
        {
            transform.localPosition = Vector2.zero;
            
            var weapons = PlayerManager.Instance.PlayerWeaponry.GetRandomWeapons(randomWeaponsCount);
            foreach (var weapon in weapons)
            {
                var slot = Instantiate(weaponSlotPrefab, weaponContainer.position, Quaternion.identity,
                    weaponContainer);
                var slotScript = slot.GetComponent<WeaponSlotUi>();
                slotScript.Setup(weapon, gameObject);
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(weaponContainer);
        }
    }
}