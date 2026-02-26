using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    [SerializeField] private bool triggerOnce = true;
    [SerializeField] private string nameTagThatThisTriggerCanUse;
    [Space]
    [SerializeField] private DelayedUnityEvent unityEvent;
    private void Start()
    {
        var a = GetComponent<MeshRenderer>();
        var b = GetComponent<MeshFilter>();
        if (a != null)
        {
            Destroy(a);
        }
        if (b != null)
        {
            Destroy(b);
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(nameTagThatThisTriggerCanUse))
        {
            unityEvent.Invoke();
            if (triggerOnce)
            {
                Destroy(this);
            }
        }
    }
}