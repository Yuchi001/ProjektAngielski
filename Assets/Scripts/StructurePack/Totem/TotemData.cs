namespace StructurePack.Totem
{
    public class TotemData
    {
        private int currentPrice = 0;
        
        public void SetCurrentPrice(int amount)
        {
            currentPrice = amount;
        }

        public void IncrementCurrentPrice(int amount)
        {
            currentPrice += amount;
        }

        public int GetCurrentPrice() => currentPrice;
    }
}