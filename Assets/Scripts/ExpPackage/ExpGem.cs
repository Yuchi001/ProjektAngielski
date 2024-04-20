using System;
using System.Collections.Generic;
using System.Linq;
using ExpPackage.Enums;
using Managers;
using PlayerPack;
using UnityEngine;

namespace ExpPackage
{
    public class ExpGem : MonoBehaviour
    {
        [SerializeField] private float animTime;
        [SerializeField] private List<ExpGemInfo> expAmountPair = new();
        private Vector3 PlayerPos => GameManager.Instance.CurrentPlayer.transform.position;
        private int expAmount = 0;

        private bool _pickedUp = false;
        
        public static void SpawnExpGem(GameObject expGemPrefab, Vector3 position, EExpGemType expGemType)
        {
            var expGem = Instantiate(expGemPrefab, position, Quaternion.identity);
            var expGemScript = expGem.GetComponent<ExpGem>();
            expGemScript.Setup(expGemType);
        }

        public void Setup(EExpGemType gemType)
        {
            var pair = expAmountPair.FirstOrDefault(e => e.gemType == gemType);
            if (pair == null) return;

            expAmount = pair.expAmount;
            GetComponent<SpriteRenderer>().sprite = pair.gemSprite;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.TryGetComponent<PlayerExp>(out var playerExp) || _pickedUp) return;

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
    }
}