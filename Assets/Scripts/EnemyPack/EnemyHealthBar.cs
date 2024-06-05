using UnityEngine;
using UnityEngine.UI;

namespace EnemyPack
{
    public class EnemyHealthBar : MonoBehaviour
    {
        [SerializeField] private float healthBarDisplayTime = 3f;
        [SerializeField] private float healthBarHidePercentage = 0.9f;
        [SerializeField] private Transform healthCanvas;
        [SerializeField] private Image healthBar;
        [SerializeField] private Image healthBarBackground;
        
        private float _healthBarTimer = 0;

        public void Setup(SpriteRenderer spriteRenderer)
        {
            healthBar.fillAmount = 1;
            
            var bounds = spriteRenderer.bounds;
            
            var healthCanvasPos = bounds.min;
            healthCanvasPos.x += bounds.size.x / 2;
            healthCanvas.position = healthCanvasPos;

            var healthBarColor = healthBar.color;
            healthBarColor.a = 0;
            healthBar.color = healthBarColor;
            
            var healthBarBackgroundColor = healthBarBackground.color;
            healthBarBackgroundColor.a = 0;
            healthBarBackground.color = healthBarBackgroundColor;
        }
        
        public void ManageHealthBar()
        {
            _healthBarTimer += Time.deltaTime;
            if (_healthBarTimer / healthBarDisplayTime < healthBarHidePercentage) return;

            var hideCap = healthBarHidePercentage * healthBarDisplayTime;
            var percentage = _healthBarTimer / hideCap / (healthBarDisplayTime / hideCap);
            var newAlpha = Mathf.Clamp(1 - percentage, 0, 1);
            var color = healthBar.color;
            color.a = newAlpha;
            healthBar.color = color;
            
            var healthBarBackgroundColor = healthBarBackground.color;
            healthBarBackgroundColor.a = newAlpha;
            healthBarBackground.color = healthBarBackgroundColor;
        }

        public void UpdateHealthBar(float currentHealth, float maxHealth)
        {
            healthBar.fillAmount = currentHealth / maxHealth;
            
            var healthBarColor = healthBar.color;
            healthBarColor.a = 1;
            healthBar.color = healthBarColor;
            
            var healthBarBackgroundColor = healthBarBackground.color;
            healthBarBackgroundColor.a = 1;
            healthBarBackground.color = healthBarBackgroundColor;
            
            _healthBarTimer = 0;
        }
    }
}