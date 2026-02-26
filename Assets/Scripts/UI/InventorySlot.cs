using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Image))]
public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public int slotIndex;
    private Image icon;

    private void Awake()
    {
        icon = GetComponent<Image>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        var item = MainMechanics.instance.player.inventory[slotIndex];
        if (item == null) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            item.UseItem(MainMechanics.instance.player);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            item.DropItem(MainMechanics.instance.player);
            icon.sprite = GameUIManager.instance.nothingSprite;
        }
    }
}
