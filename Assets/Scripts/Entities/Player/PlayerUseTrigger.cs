using UnityEngine;

public class PlayerUseTrigger : MonoBehaviour
{
    [SerializeField] private Player player;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.isStatic) return;
        if (!other.CompareTag(nameof(Usable))) return;
        
        var usableComponent = other.GetComponentInParent<Usable>();
        if (usableComponent != null)
            player.useList.Add(other, usableComponent);
    }

    private void OnTriggerExit(Collider other) => player.useList.Remove(other);
}
