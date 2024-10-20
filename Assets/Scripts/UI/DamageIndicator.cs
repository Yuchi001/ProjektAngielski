using Managers;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UI
{
    public class DamageIndicator : MonoBehaviour
    {
        [SerializeField] private Gradient damageColorGradient;
        [SerializeField] private Color healColor;
        [SerializeField] private TextMeshProUGUI damageText;
        
        private void Setup(int damage, bool isDamage, bool isCrit)
        {
            var scaledDamage = Mathf.Clamp(damage - 5, 0, 50);
            damageText.color = isDamage ? damageColorGradient.Evaluate(scaledDamage / 50f) : healColor;
            if (isCrit) damageText.color = Color.magenta;
            damageText.text = !isCrit ? damage.ToString() : $"<i>{damage}</i>";
            
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