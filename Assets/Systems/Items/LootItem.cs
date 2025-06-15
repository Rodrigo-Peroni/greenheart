namespace Assets.Systems.Items
{
    /// <summary>
    /// This class relates Items and the respective amount to be looted in a single entity.    
    /// </summary>
    public class LootItem
    {
        public ItemCode Code { get; set; }
        public int Amount { get; set; }

        public LootItem(ItemCode code, int amount)
        {
            this.Code = code;
            this.Amount = amount;
        }
    }
}
