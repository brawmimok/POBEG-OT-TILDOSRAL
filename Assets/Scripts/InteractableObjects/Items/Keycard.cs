public class Keycard : InventoryItem
{
    public int KeyCardLevel;
    public override void GetItemComponent(Player activator)
    {
        base.GetItemComponent(activator);
        for (int i = 0; i < activator.inventory.Length; i++)
        {
            if (activator.inventory[i] is Keycard keycard && keycard.KeyCardLevel > activator.keyCardLevel)
            {
                activator.keyCardLevel = keycard.KeyCardLevel;
            }
        }
    }
    public override void DropItem(Player activator)
    {
        base.DropItem(activator);
        activator.keyCardLevel = 0;
        for (int i = 0; i < activator.inventory.Length; i++)
        {
            if (activator.inventory[i] is Keycard keycard && keycard.KeyCardLevel > activator.keyCardLevel)
            {
                activator.keyCardLevel = keycard.KeyCardLevel;
            }
        }
    }
    public override void UseItem(Player player)
    {
        base.UseItem(player);
        GameUIManager.instance.ShowText("Карта сама юзнётся, если подходит к двери");
    }
}