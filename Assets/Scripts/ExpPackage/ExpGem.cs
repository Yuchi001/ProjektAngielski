using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnchantmentPack.Enums;
using ExpPackage.Enums;
using Managers;
using Managers.Enums;
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
        [SerializeField] private List<ExpGemInfo> betterGemAmountPair = new();
        private static Vector3 PlayerPos => GameManager.Instance.CurrentPlayer.transform.position;

        private static PlayerEnchantments PlayerEnchantments =>
            PlayerManager.Instance.PlayerEnchantments;
        
        private int expAmount = 0;

        private float _timer = 0;

        private EPickableState _pickableState = EPickableState.Default;
        private Vector2 startPosition;
        
        public static void SpawnExpGem(GameObject expGemPrefab, Vector3 position, EExpGemType expGemType)
        {
            var expGem = Instantiate(expGemPrefab, position, Quaternion.identity);
            var expGemScript = expGem.GetComponent<ExpGem>();
            expGemScript.Setup(expGemType);
        }

        private void Setup(EExpGemType gemType)
        {
            var gemList = PlayerEnchantments.Has(EEnchantmentName.BetterExp)
                ? betterGemAmountPair
                : expAmountPair;
            var pair = gemList.FirstOrDefault(e => e.gemType == gemType);
            if (pair == null) return;

            expAmount = pair.expAmount;
            GetComponent<SpriteRenderer>().sprite = pair.gemSprite;
            startPosition = transform.position;
        }

        private void Update()
        {
            if (PlayerManager.Instance == null) return;

            switch (_pickableState)
            {
                case EPickableState.Default:
                    if (Vector2.Distance(transform.position, PlayerPos) > range) return;

                    _pickableState = EPickableState.PickingUpPhase;
                    return;
                case EPickableState.PickingUpPhase:
                    _timer += Time.deltaTime;
                    var remainingTime = Mathf.Clamp01(_timer / animTime);

                    transform.position = Vector3.Lerp(startPosition, PlayerPos, remainingTime);
                    
                    if (Vector2.Distance(transform.position, PlayerPos) > 0.1f) return;
                    
                    AudioManager.Instance.PlaySound(ESoundType.PickUpGem);

                    _pickableState = EPickableState.PickedUp;
                    
                    var playerExp = PlayerManager.Instance.PlayerExp;
                    playerExp.GainExp(expAmount);
                    Destroy(gameObject);
                    return;
                case EPickableState.PickedUp: return;
                default: return;
            }
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, range);
        }

        public enum EPickableState
        {
            Default,
            PickingUpPhase,
            PickedUp,
        }
    }
}