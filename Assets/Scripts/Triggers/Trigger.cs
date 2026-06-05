using UnityEngine;
using UnityEngine.Events;

public class Trigger : PepusBehaviour
{
    [Header("Trigger Settings")]
    [SerializeField] private bool triggerOnce = true;
    [SerializeField] private string nameTagThatThisTriggerCanUse;
    [Space]
    [SerializeField] private DelayedUnityEvent unityEvent;

    private static string PrefabResourcePath => "SEXPREFAB";

    [Saveable][HideInInspector] public float savedTimeElapsed;
    [Saveable][HideInInspector] public Vector3 colliderCenter;
    [Saveable][HideInInspector] public Vector3 colliderSize;

    private void Awake()
    {
        // Удаляем сетку ProBuilder-куба при запуске
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

    public override void OnBeforeSave()
    {
        base.OnBeforeSave();
        savedTimeElapsed = unityEvent.timeElapsed;

        BoxCollider box = GetComponent<BoxCollider>();
        if (box != null)
        {
            colliderCenter = box.center;
            colliderSize = box.size;
        }
    }

    public override void OnAfterLoad()
    {
        base.OnAfterLoad();

        BoxCollider box = GetComponent<BoxCollider>();
        box.isTrigger = true;
        box.center = colliderCenter;
        box.size = colliderSize;

        if (savedTimeElapsed > 0f)
        {
            unityEvent.timeElapsed = savedTimeElapsed;
            unityEvent.Invoke();
        }
    }
}
