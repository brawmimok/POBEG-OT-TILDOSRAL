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
    protected override void Awake()
    {
        base.Awake();
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
    public virtual void DropItem(Player activator) // Лучше не заменять (А я ща нахуй всё удалю, а не заменю)
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
    [Serializable]
    public class InvItemSaveData : SaveData
    {
        public bool mr;
        public bool coll;
        public bool rb;

        public float x, y, z;

        public float wRot, xRot, yRot, zRot;
    }
    public override void OnSave()
    {
        saveData = new InvItemSaveData()
        {
            x = transform.position.x,
            y = transform.position.y,
            z = transform.position.z,

            wRot = transform.rotation.w,
            xRot = transform.rotation.x,
            yRot = transform.rotation.y,
            zRot = transform.rotation.z,

            mr = mr.enabled,
            coll = coll.enabled,
            rb = rb.useGravity,

            active = gameObject.activeInHierarchy,
            id = Id
        };
    }
    public override void OnLoad(SaveData saveData)
    {
        base.OnLoad(saveData);
        InvItemSaveData saveData1 = (InvItemSaveData)saveData;

        transform.position = new Vector3(saveData1.x, saveData1.y, saveData1.z);
        transform.rotation = new Quaternion(saveData1.xRot, saveData1.yRot, saveData1.zRot, saveData1.wRot);
        mr.enabled = saveData1.mr;
        coll.enabled = saveData1.coll;
        rb.useGravity = saveData1.rb;
    }
}