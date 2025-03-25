using System.Collections.Generic;
using ItemPack.SO;
using PlayerPack;
using UnityEngine;

namespace ItemPack.WeaponPack.Other
{
    public class ForceField : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer fieldSprite;
        
        private bool _ready = false;
        private string _weaponName;

        private float _attackSpeed;
        private float _scale;
        private float _animSpeed;

        private int _currentIndex;

        private List<Sprite> _sprites = new();

        private float _timer = 0;

        #region Setup methods

        public ForceField Setup(string weaponName)
        {
            _weaponName = weaponName;
            return this;
        }

        public ForceField SetFieldSprite(Sprite sprite)
        {
            _sprites = new List<Sprite> {sprite};
            return this;
        }

        public ForceField SetFieldSprite(List<Sprite> sprites, float animSpeed)
        {
            _animSpeed = animSpeed;
            _sprites = sprites;
            return this;
        }

        public void SetReady()
        {
            _ready = true;
        }            

        #endregion

        private void Update()
        {
            if (!_ready) return;
            
            AnimateForceField();
        }
        
        private void AnimateForceField()
        {
            _timer += Time.deltaTime;
            if (_timer < 1 / _animSpeed || _sprites.Count <= 1) return;

            _timer = 0;
            _currentIndex++;
            if (_currentIndex >= _sprites.Count) _currentIndex = 0;

            fieldSprite.sprite = _sprites[_currentIndex];
        }

        private void OnResetStats(SoInventoryItem inventoryItem)
        {
            if (inventoryItem.ItemName != _weaponName) return;
            
            Destroy(gameObject);
        }
    }
}