using System.Collections.Generic;
using System.Linq;
using EffectPack.SO;
using Other;
using Other.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace EffectPack
{
    public class EffectStatusObject : MonoBehaviour
    {
        private List<SoEffectBase> _effectsDisplaySettings;
        
        private Image _effectImage;

        private int _damageStacks = 0;

        private SoEffectBase _pickedEffectBase;
        private ParticleSystem _spawnedParticles;

        private float _resolveTimer = 0;
        private float _effectTimer = 0;

        private EffectsManager _effectsManager;
        private CanBeDamaged _canBeDamaged;

        private Transform _transform;

        private bool _hasParticles;
        
        private void Awake()
        {
            _transform = transform;
            
            _effectsDisplaySettings = Resources.LoadAll<SoEffectBase>("EffectStatus").ToList();
            _effectImage = GetComponent<Image>();
            
            gameObject.SetActive(false);
        }

        public void SpawnSetup(EEffectType effectType, EffectsManager effectsManager, CanBeDamaged canBeDamaged)
        {
            transform.position = Vector3.zero;
            _effectsManager = effectsManager;
            _canBeDamaged = canBeDamaged;
            
            _damageStacks = 0;
            _resolveTimer = 0;
            _effectTimer = 0;

            _pickedEffectBase = _effectsDisplaySettings.FirstOrDefault(e => e.EffectType == effectType);
            if (_pickedEffectBase == default) return;

            _hasParticles = _pickedEffectBase.EffectParticles != null;
            
            _effectImage.sprite = _pickedEffectBase.EffectSprite;

            if (_hasParticles)
            {
                _spawnedParticles = Instantiate(_pickedEffectBase.EffectParticles, transform);
                _spawnedParticles.Clear();
                _spawnedParticles.Stop();
                _spawnedParticles.transform.position = Vector3.zero;
            }

            gameObject.SetActive(false);
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
            _pickedEffectBase.OnAdd(_effectsManager, _damageStacks, _canBeDamaged);
            
            gameObject.SetActive(true);
            _damageStacks++;

            _effectTimer = Mathf.Max(_effectTimer, 0) + effectLength;
            
            if (_pickedEffectBase.IsCountinues && _hasParticles) _spawnedParticles.Play();
        }

        private void Update()
        {
            _effectTimer -= Time.deltaTime;
            if (_effectTimer < 0)
            {
                if (_pickedEffectBase.IsCountinues) _pickedEffectBase.OnResolve(_effectsManager, _damageStacks, _canBeDamaged);
                gameObject.SetActive(false);
                return;
            }

            if (_pickedEffectBase.IsCountinues) return;

            _resolveTimer += Time.deltaTime;
            if (_resolveTimer < _pickedEffectBase.ResolveRate) return;
            _resolveTimer = 0;

            _pickedEffectBase.OnResolve(_effectsManager, _damageStacks, _canBeDamaged);
            if (_hasParticles) _spawnedParticles.Play();
        }

        [System.Serializable]
        public struct EffectStatus
        {
            public EEffectType effectType;
            public Sprite effectStatusSprite;
            public Color effectColor;
        }
    }
}