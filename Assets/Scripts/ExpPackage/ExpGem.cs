using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private static Vector3 PlayerPos => GameManager.Instance.CurrentPlayer.transform.position;
        private int expAmount = 0;

        private float _timer = 0;

        private EGemState _gemState = EGemState.Default;
        private Vector2 startPosition;
        
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
            startPosition = transform.position;
        }

        private void Update()
        {
            if (PlayerManager.Instance == null) return;

            switch (_gemState)
            {
                case EGemState.Default:
                    if (Vector2.Distance(transform.position, PlayerPos) > range) return;

                    _gemState = EGemState.PickingUpPhase;
                    return;
                case EGemState.PickingUpPhase:
                    _timer += Time.deltaTime;
                    var remainingTime = Mathf.Clamp01(_timer / animTime);

                    transform.position = Vector3.Lerp(startPosition, PlayerPos, remainingTime);
                    
                    if (Vector2.Distance(transform.position, PlayerPos) > 0.1f) return;
                    
                    AudioManager.Instance.PlaySound(ESoundType.PickUpGem);

                    _gemState = EGemState.PickedUp;
                    
                    var playerExp = PlayerManager.Instance.PlayerExp;
                    playerExp.GainExp(expAmount);
                    Destroy(gameObject);
                    return;
                case EGemState.PickedUp: return;
                default: return;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, range);
        }

        private enum EGemState
        {
            Default,
            PickingUpPhase,
            PickedUp,
        }
    }
}