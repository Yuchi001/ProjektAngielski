using System;
using System.Collections.Generic;
using System.Linq;
using EffectPack.SO;
using Other;
using Other.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EffectPack
{
    public class EffectStatusObject : MonoBehaviour
    {
        private Image _effectImage;
        private TextMeshProUGUI _effectDamageStackText;

        private int _damageStacks = 0;

        private SoEffectBase _pickedEffectBase;
        public SoEffectBase EffectBase => _pickedEffectBase;
        
        private ParticleSystem _spawnedParticles;

        private float _resolveTimer = 0;
        private float _effectTimer = 0;

        private EffectsManager _effectsManager;
        private CanBeDamaged _canBeDamaged;

        private Transform _transform;

        private bool _hasParticles;
        
        public bool IsActive { get; private set; }

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void SpawnSetup(SoEffectBase effectBase, EffectsManager effectsManager, CanBeDamaged canBeDamaged)
        {
            _transform = transform;
            
            _effectImage = GetComponent<Image>();
            
            _effectDamageStackText = GetComponentInChildren<TextMeshProUGUI>();
            
            transform.position = Vector3.zero;
            _effectsManager = effectsManager;
            _canBeDamaged = canBeDamaged;
            
            _damageStacks = 0;
            _resolveTimer = 0;
            _effectTimer = 0;

            _effectDamageStackText.text = "";

            _pickedEffectBase = effectBase;
            if (_pickedEffectBase == default) return;

            _hasParticles = _pickedEffectBase.EffectParticles != null;
            
            _effectImage.sprite = _pickedEffectBase.EffectSprite;

            if (!_hasParticles) return;
            
            _spawnedParticles = Instantiate(_pickedEffectBase.EffectParticles, transform);
            _spawnedParticles.Clear();
            _spawnedParticles.Stop();
            _spawnedParticles.transform.position = Vector3.zero;
        }
        
        public void Setup()
        {
            _transform.position = Vector3.zero;
            
            _damageStacks = 0;
            _resolveTimer = 0;
            _effectTimer = 0;
            
            if (_hasParticles)
            {
                _spawnedParticles.Clear();
                _spawnedParticles.Stop();
                _spawnedParticles.transform.position = Vector3.zero;
            }
            
            gameObject.SetActive(false);
        }

        public void StackEffect(float effectLength)
        {
            IsActive = true;
            
            _pickedEffectBase.OnAdd(_effectsManager, _damageStacks, _canBeDamaged);
            
            gameObject.SetActive(true);
            if (_pickedEffectBase.CanStack || _damageStacks == 0) _damageStacks++;

            _effectDamageStackText.text = _damageStacks < 1 ? "" : $"x{_damageStacks}";
            
            _effectTimer = Mathf.Max(_effectTimer, 0) + effectLength;

            _canBeDamaged.SpriteRenderer.color = _pickedEffectBase.EffectColor;
            
            if (_pickedEffectBase.IsCountinues && _hasParticles) _spawnedParticles.Play();
        }

        private void Update()
        {
            _effectTimer -= Time.deltaTime;
            if (_effectTimer < 0)
            {
                if (_pickedEffectBase.IsCountinues) _pickedEffectBase.OnResolve(_effectsManager, _damageStacks, _canBeDamaged);
                gameObject.SetActive(false);
                IsActive = false;
                _canBeDamaged.SpriteRenderer.color = _effectsManager.TryGetAnyEffect(out var effect) ? effect.EffectColor : Color.white;
                return;
            }

            if (_pickedEffectBase.IsCountinues) return;

            _resolveTimer += Time.deltaTime;
            if (_resolveTimer < _pickedEffectBase.ResolveRate) return;
            _resolveTimer = 0;

            _pickedEffectBase.OnResolve(_effectsManager, _damageStacks, _canBeDamaged);
            if (_hasParticles) _spawnedParticles.Play();
        }
    }
}