using System;
using System.Collections.Generic;
using System.Linq;
using ExpPackage.Enums;
using Managers;
using PlayerPack;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace ExpPackage
{
    public class ExpGem : MonoBehaviour
    {
        [SerializeField] private Light2D light2D;
        [SerializeField] private float range;
        [SerializeField] private float animTime;
        [SerializeField] private List<ExpGemInfo> expAmountPair = new();
        private Vector3 PlayerPos => GameManager.Instance.CurrentPlayer.transform.position;
        private int expAmount = 0;

        private bool _pickedUp = false;
        
        private bool _animating = false;

        private const float minStrFallOff = 0.7f;
        
        public static void SpawnExpGem(GameObject expGemPrefab, Vector3 position, EExpGemType expGemType)
        {
            var expGem = Instantiate(expGemPrefab, position, Quaternion.identity);
            var expGemScript = expGem.GetComponent<ExpGem>();
            expGemScript.Setup(expGemType);
        }

        private void Setup(EExpGemType gemType)
        {
            var pair = expAmountPair.FirstOrDefault(e => e.gemType == gemType);
            if (pair == null) return;

            expAmount = pair.expAmount;
            GetComponent<SpriteRenderer>().sprite = pair.gemSprite;
        }

        private void Update()
        {
            if (PlayerManager.Instance == null) return;

            ManageAnimation();
            
            if (Vector2.Distance(transform.position, PlayerPos) > range || _pickedUp) return;

            PickUp();
        }

        private void ManageAnimation()
        {
            if (_animating) return;

            _animating = true;
            
            if (light2D.falloffIntensity <= minStrFallOff)
            {
                LeanTween.value(light2D.falloffIntensity, 1, 1f)
                    .setOnUpdate((float val) =>
                    {
                        light2D.falloffIntensity = val;
                    })
                    .setOnComplete(() => _animating = false);
                return;
            }
            
            LeanTween.value(1, minStrFallOff - 0.05f, 1f)
                .setOnUpdate((float val) =>
                {
                    light2D.falloffIntensity = val;
                })
                .setOnComplete(() => _animating = false);
        }

        private void PickUp()
        {
            var playerExp = PlayerManager.Instance.PlayerExp;
            
            _pickedUp = true;
            var tween = LeanTween.move(gameObject, PlayerPos, animTime)
                .setEaseInExpo()
                .setOnComplete(() =>
                {
                    if (playerExp == null) return;
                
                    playerExp.GainExp(expAmount);
                    Destroy(gameObject);
                });
            tween.setOnUpdate((float val) => tween.setTo(PlayerPos));
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}