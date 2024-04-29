using UnityEngine;

namespace Other.SO
{
    [CreateAssetMenu(fileName = "new Food", menuName = "Custom/Food")]
    public class SoFood : ScriptableObject
    {
        [SerializeField] private int saturationValue;
        [SerializeField] private Sprite foodSprite;

        public int SaturationValue => saturationValue;
        public Sprite FoodSprite => foodSprite;
    }
}