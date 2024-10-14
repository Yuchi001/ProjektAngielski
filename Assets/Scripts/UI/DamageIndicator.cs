﻿using Managers;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UI
{
    public class DamageIndicator : MonoBehaviour
    {
        [SerializeField] private Gradient damageColorGradient;
        [SerializeField] private Color healColor;
        [SerializeField] private float animTime = 0.3f;
        [SerializeField] private float moveValue = 0.5f;
        [SerializeField] private TextMeshProUGUI damageText;
        
        //TODO: CO ROBI LEAN TWEEN W TAKIM MIEJSCU TO POZERA TONE ZASOBOW KUTWA
        //TODO: USTAW TU ANIMACJE Z SYSTEMU ANIMACJI UNITY!
        //TODO: LEPIEJ POKAŻ ŻE TO KRYT
        private void Setup(int damage, bool isDamage, bool isCrit)
        {
            var scaledDamage = Mathf.Clamp(damage - 5, 0, 50);
            damageText.color = isDamage ? damageColorGradient.Evaluate(scaledDamage / 50f) : healColor;
            if (isCrit) damageText.color = Color.magenta;
            damageText.text = !isCrit ? damage.ToString() : $"<i>{damage}</i>";
            
            LeanTween.value(1, 0, animTime)
                .setEaseInExpo()
                .setOnUpdate((float val) =>
                {
                    var color = damageText.color;
                    color.a = val;
                    damageText.color = color;
                });

            var movePos = transform.position;
            movePos.y += moveValue;
            LeanTween.move(gameObject, movePos, animTime / 2).setEaseOutQuint();

            LeanTween.scaleX(gameObject, 0, animTime).setEaseInBack();
            LeanTween.scaleY(gameObject, 0, animTime).setEaseInBack();
            
            Destroy(gameObject, 2f);
        }
        
        public static void SpawnDamageIndicator(Vector2 position, GameObject indicatorPrefab, int value, bool isCrit, bool isDamage = true)
        {
            var randomX = Random.Range(-0.1f, 0.15f);
            position.x += randomX;
            var indicator = Instantiate(
                indicatorPrefab, 
                position, 
                Quaternion.identity, 
                GameUiManager.Instance.WorldCanvas);
            var indicatorScript = indicator.GetComponent<DamageIndicator>();
            indicatorScript.Setup(value, isDamage, isCrit);
        }
    }
}