using System;

public class Keycard : InventoryItem
{
    public int KeyCardLevel;
    private void CalculatePlayerKeyCardLevel(Player player)
    {
        for (int i = 0; i < player.inventory.Length; i++)
            if (player.inventory[i] is Keycard keycard && keycard.KeyCardLevel > player.keyCardLevel)
                player.keyCardLevel = keycard.KeyCardLevel;
    }
    public override void GetItemComponent(Player activator)
    {
        base.GetItemComponent(activator);
        CalculatePlayerKeyCardLevel(activator);
    }
    public override void DropItem(Player activator)
    {
        base.DropItem(activator);
        activator.keyCardLevel = 0;
        CalculatePlayerKeyCardLevel(activator);
    }
    public override void UseItem(Player player)
    {
        base.UseItem(player);
        GameUIManager.instance.ShowText("Карта сама юзнётся, если подходит к двери");
    }
}