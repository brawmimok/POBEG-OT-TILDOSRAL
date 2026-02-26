using UnityEngine;

public class Displayable : InventoryItem
{
    public Sprite displayContent;
    public override void UseItem(Player activator)
    {
        base.UseItem(activator);
        GameUIManager.instance.DisplayItemOnScreen(displayContent);
    }
}