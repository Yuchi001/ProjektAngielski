using System.Collections.Generic;
using UnityEngine;

namespace WeaponPack.SO
{
    [CreateAssetMenu(fileName = "new Weapon", menuName = "Custom/Weapon")]
    public class SoWeapon : ScriptableObject
    {
        [SerializeField] private string weaponName;
        [SerializeField] private Sprite weaponSprite;
        [SerializeField] private bool oneTimeSpawnLogic = false;
        [SerializeField] private GameObject weaponLogicPrefab;

        [SerializeField] private List<WeaponStatPair> weaponStartingStats;
        [SerializeField, Tooltip("Each index will have ")] private List<UpgradeWeaponStats> weaponUpgradeStats;

        public string WeaponName => weaponName;
        public bool OneTimeSpawnLogic => oneTimeSpawnLogic;
        public GameObject WeaponLogicPrefab => weaponLogicPrefab;
        public Sprite WeaponSprite => weaponSprite;
        public List<WeaponStatPair> WeaponStartingStats => weaponStartingStats;
        public List<UpgradeWeaponStats> WeaponUpgradeStats => weaponUpgradeStats;
    }
}