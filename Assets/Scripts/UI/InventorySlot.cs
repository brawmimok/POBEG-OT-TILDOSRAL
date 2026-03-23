using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int slotIndex;
    public Image redBorder;
    public Image itemImage;
    public TextMeshProUGUI itemName;
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
            itemImage.sprite = GameUIManager.instance.nothingSprite;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"Slot {slotIndex}: redBorder={redBorder}, itemName={itemName}, itemImage={itemImage}");
        var item = MainMechanics.instance.player.inventory[slotIndex];
        if (item == null) return;
        redBorder.enabled = true;
        itemName.text = item.itemName;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        redBorder.enabled = false;
        itemName.text = "";
    }
}
