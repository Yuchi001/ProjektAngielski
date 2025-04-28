using ShopPack;
using UIPack;
using UnityEngine;

namespace StructurePack.SO
{
    [CreateAssetMenu(fileName = "new Shop", menuName = "Custom/Structure/Shop")]
    public class SoShop : SoStructure
    {
        private const string UI_KEY = "SHOP_UI";
        
        public override bool OnInteract(StructureBase structureBase)
        {
            if (UIManager.IsOpen(UI_KEY))
            {
                UIManager.CloseUI(UI_KEY);
                return true;
            }
            
            UIManager.OpenUI<ShopUI>(UI_KEY, GetOpenStrategy(structureBase), GetCloseStrategy());
            return true;
        }
    }
}