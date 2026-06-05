using System;
using UnityEngine;

public abstract class InventoryItem : Usable
{
    public string itemName;
    public Sprite icon;

    public AudioClip pickUpSound;
    public AudioClip useSound;
    public AudioClip putOutSound;
    [NonSerialized] public sbyte slotIndex = -1;

    private Rigidbody rb;
    private Collider coll;
    private MeshRenderer mr;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
        mr = GetComponent<MeshRenderer>();
    }
    //public abstract InventoryItem NewInventoryItem();
    override public void Use(Player activator) //Pick Up item
    {
        for (sbyte i = 0; i < activator.inventory.Length; i++)
        {
            if (activator.inventory[i] == null)
            {
                mr.enabled = false;
                rb.useGravity = false;
                coll.enabled = false;

                slotIndex = i;
                GetItemComponent(activator);
                MainMechanics.instance.uiAudio.PlayOneShot(pickUpSound);
                activator.useList.Remove(GetComponent<Collider>());
                return;
            }
        }
    }
    public virtual void GetItemComponent(Player activator)
    {
        activator.inventory[slotIndex] = this;
    }
    public virtual void UseItem(Player player)
    {
        MainMechanics.instance.uiAudio.PlayOneShot(useSound);
        GameUIManager.instance.CloseInventory();
    }
    public virtual void DropItem(Player activator) // Лучше не заменять (хоть и стоит virtual, нужно быть аккуратнее) (А я ща нахуй всё удалю, а не заменю)
    {
        MainMechanics.instance.uiAudio.PlayOneShot(putOutSound);

        transform.SetPositionAndRotation(
            activator.transform.position, Quaternion.identity);
        mr.enabled = true;
        coll.enabled = true;
        rb.useGravity = true;

        activator.inventory[slotIndex] = null;
        GameUIManager.instance.CloseInventory();
        slotIndex = -1;
    }
}