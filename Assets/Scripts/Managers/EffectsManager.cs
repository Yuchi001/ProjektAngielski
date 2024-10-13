using System.Collections.Generic;
using System.Linq;
using EnchantmentPack.Enums;
using Other;
using Other.Enums;
using PlayerPack;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Managers
{
    public class EffectsManager : MonoBehaviour
    {
        [SerializeField] private GameObject statusListObject;
        [SerializeField] private RectTransform statusListHolder;
        [SerializeField] private GameObject statusImagePrefab;
        [SerializeField] private List<EffectStatus> _effectStatusList = new();

        [SerializeField] private int poisonDamage = 10;
        [SerializeField] private int burnDamage = 5;
        [SerializeField] private int bleedDamage = 1;
        
        [FormerlySerializedAs("damageRate")] [SerializeField] private int effectResolveRate = 2;
        
        [SerializeField] private GameObject poisonParticles;
        [SerializeField] private GameObject burnParticles;
        [SerializeField] private GameObject slowParticles;
        
        private CanBeDamaged _canBeDamaged;
        
        private readonly List<EffectInfo> _effectList = new();
        private readonly List<EffectActiveObjects> _effectSpawnedObjects = new();

        private static PlayerEnchantmentManager PlayerEnchantmentManager =>
            PlayerManager.Instance.PlayerEnchantmentManager;
        
        public bool Stuned { get; private set; }
        public bool Slowed { get; private set; }

        private bool _ready = false;
        private float _timer = 0;
        
        private readonly List<EffectInfo> _effectListQueue = new();

        public void Setup(CanBeDamaged canBeDamaged)
        {
            _canBeDamaged = canBeDamaged;
            _ready = canBeDamaged != null;

            var spriteRender = canBeDamaged.SpriteRenderer;
            var bounds = spriteRender.bounds;
            
            var center = bounds.center;
            var topRight = bounds.max;
            var topCenter = topRight;
            topCenter.x = center.x;
            topCenter.y += 0.1f;
            
            var scale = statusListObject.transform.localScale.x / canBeDamaged.transform.localScale.x;
            statusListObject.transform.localScale = new Vector3(scale, scale, scale);

            statusListObject.transform.position = topCenter;
        }

        public void AddEffect(EEffectType effectType, float time)
        {
            if(!_ready) return;
            
            if (_effectListQueue.FirstOrDefault(e => e.effectType == effectType) != null) return;
            
            var effectColor = _effectStatusList.FirstOrDefault(s => s.effectType == effectType).effectColor;
            _canBeDamaged.SpriteRenderer.color = effectColor;
            
            switch (effectType)
            {
                case EEffectType.Stun:
                    Stuned = true;
                    break;
                case EEffectType.Slow:
                    var slowParticlesInstance = Instantiate(slowParticles, _canBeDamaged.transform.position,
                        Quaternion.identity, _canBeDamaged.transform);
                    _effectSpawnedObjects.Add(new EffectActiveObjects
                    {
                        effectType = EEffectType.Slow,
                        spawnedObjects = new List<GameObject>{slowParticlesInstance},
                    });
                    Slowed = true;
                    break;
            }

            var stacks = 0;
            var effect = _effectList.FirstOrDefault(e => e.effectType == effectType);
            if (effect != null)
            {
                stacks = effect.stacks;
                Destroy(effect.spawnedStatus);
                _effectList.Remove(effect);
            }

            stacks++;
            
            var status = Instantiate(statusImagePrefab, statusListHolder.position, Quaternion.identity, statusListHolder);
            var effectSprite = _effectStatusList.FirstOrDefault(s => s.effectType == effectType).effectStatusSprite;
            status.GetComponent<Image>().sprite = effectSprite;
            var statusStacks = status.GetComponentInChildren<TextMeshProUGUI>();
            var stackable = Stackable(effect);
            statusStacks.gameObject.SetActive(stackable);
            if(Stackable(effect))
                statusStacks.text = $"x{stacks}";
            
            _effectListQueue.Add(new EffectInfo
            {
                effectType = effectType, 
                time = time,
                spawnedStatus = status,
                stacks = stacks,
            });
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(statusListHolder);
        }

        private static bool Stackable(EffectInfo effectInfo)
        {
            if (effectInfo == null) return false;

            return (effectInfo.effectType == EEffectType.Bleed || PlayerEnchantmentManager.Has(EEnchantmentName.PoisonCanStack)) && effectInfo.stacks > 1;
        }

        private EffectStatus? GetEffectStatus(EEffectType effectType)
        {
            return _effectStatusList.FirstOrDefault(e => e.effectType == effectType);
        }

        private void ResolveEffect(EffectInfo effectInfo)
        {
            var effectStatus = GetEffectStatus(effectInfo.effectType);
            if (effectStatus == null) return;

            var entityPos = _canBeDamaged.transform.position;

            switch (effectInfo.effectType)
            {
                case EEffectType.Poison:
                    if(_canBeDamaged.CurrentHealth == 1) break;
                    var poisonParticlesInstance = Instantiate(poisonParticles, entityPos, Quaternion.identity);
                    Destroy(poisonParticlesInstance, 2f);
                    var poisonDamageWithStacks = PlayerEnchantmentManager.Has(EEnchantmentName.PoisonCanStack)
                        ? poisonDamage * effectInfo.stacks
                        : poisonDamage;
                    var calculatedPoisonDamage = poisonDamageWithStacks + _canBeDamaged.MaxHealth / 50;
                    if (calculatedPoisonDamage > _canBeDamaged.CurrentHealth + 1) calculatedPoisonDamage = _canBeDamaged.CurrentHealth - 1;
                    _canBeDamaged.GetDamaged(calculatedPoisonDamage, effectStatus.Value.effectColor);
                    break;
                case EEffectType.Burn:
                    var burnParticlesInstance = Instantiate(burnParticles, entityPos, Quaternion.identity);
                    Destroy(burnParticlesInstance, 2f);
                    _canBeDamaged.GetDamaged(burnDamage + _canBeDamaged.MaxHealth / 100, effectStatus.Value.effectColor);
                    break;
                case EEffectType.Bleed:
                    var calculatedBleedDamage = bleedDamage * effectInfo.stacks;
                    if (PlayerEnchantmentManager.Has(EEnchantmentName.BleedStacking))
                        calculatedBleedDamage += PlayerEnchantmentManager.GetStacks(EEnchantmentName.BleedStacking);
                    _canBeDamaged.GetDamaged(calculatedBleedDamage, effectStatus.Value.effectColor);
                    break;
            }
        }

        public void RemoveEffect(EEffectType effectType)
        {
            if(!_ready) return;
            
            var effectSpawnedObjects = _effectSpawnedObjects.FirstOrDefault(e => e.effectType == effectType);
            effectSpawnedObjects?.spawnedObjects.ForEach(Destroy);

            var effectInfo = _effectList.FirstOrDefault(e => e.effectType == effectType);
            Destroy(effectInfo?.spawnedStatus);
            
            switch (effectType)
            {
                case EEffectType.Stun:
                    Stuned = false;
                    break;
                case EEffectType.Slow:
                    Slowed = false;
                    break;
            }
        }

        private void Update()
        {
            if(!_ready) return;

            _timer += Time.deltaTime;
            
            var removedEffects = new List<EffectInfo>();
            _effectList.ForEach(e =>
            {
                e.time -= Time.deltaTime;
                if(e.time <= 0) removedEffects.Add(e);
            });
            
            removedEffects.ForEach(e =>
            {
                RemoveEffect(e.effectType);
                _effectList.Remove(e);
            });

            EEffectType? effectType = _effectList.Count > 0 ? _effectList[0].effectType : null;
            var effectColor = effectType == null ? Color.white : _effectStatusList.FirstOrDefault(s => s.effectType == effectType).effectColor;
            _canBeDamaged.SpriteRenderer.color = effectColor;

            if (_timer <= 1f / effectResolveRate) return;

            _timer = 0;
            
            _effectList.AddRange(_effectListQueue);
            _effectList.ForEach(ResolveEffect);
            _effectListQueue.Clear();
        }
    }
    
    public class EffectInfo
    {
        public EEffectType effectType;
        public float time;
        public int stacks;
        public GameObject spawnedStatus;
    }

    public class EffectActiveObjects
    {
        public EEffectType effectType;
        public List<GameObject> spawnedObjects = new();
    }

    [System.Serializable]
    public struct EffectStatus
    {
        public EEffectType effectType;
        public Sprite effectStatusSprite;
        public Color effectColor;
    }
}