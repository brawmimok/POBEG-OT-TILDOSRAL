using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Events.DelayedUnityEvent;

public class Trigger : PepusBehaviour
{
    [Header("Trigger Settings")]
    [SerializeField] private bool triggerOnce = true;
    [SerializeField] private string nameTagThatThisTriggerCanUse;
    [Space]
    [SerializeField] private DelayedUnityEvent unityEvent;

    protected override void Awake()
    {
        base.Awake();
        var a = GetComponent<MeshRenderer>();
        var b = GetComponent<MeshFilter>();
        if (a != null) Destroy(a);
        if (b != null) Destroy(b);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(nameTagThatThisTriggerCanUse))
        {
            unityEvent.Invoke();
            if (triggerOnce)
                gameObject.SetActive(false);
        }
    }
    public override void OnSave()
    {
        saveData = new DelayedSaveData()
        {
            timeElapsed = unityEvent.timeElapsed,

            active = gameObject.activeInHierarchy,
            id = Id
        };
    }
    public override void OnLoad(SaveData saveData)
    {
        base.OnLoad(saveData);
        DelayedSaveData saveData1 = (DelayedSaveData)saveData;

        unityEvent.timeElapsed = saveData1.timeElapsed;
        if (unityEvent.timeElapsed > 0f)
            unityEvent.Invoke();
    }
}