using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EnchantmentPack
{
    /// <summary>
    /// This class only coontains data of ui elements that can be accessed by Enchantment Display Strategies,
    /// each of the ui elements should be disabled and hooked here
    /// </summary>
    public class EnchantmentItemAccessor : MonoBehaviour
    {
        [SerializeField] private Image mainImage;
        [SerializeField] private Image secondaryImage;
        [SerializeField] private TextMeshProUGUI cornerText;
        [SerializeField] private TextMeshProUGUI mainText;

        public Image MainImage => mainImage;
        public Image SecondaryImage => secondaryImage;
        public TextMeshProUGUI CornerText => cornerText;
        public TextMeshProUGUI MainText => mainText;
    }
}