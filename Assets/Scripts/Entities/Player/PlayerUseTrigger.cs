using UnityEngine;

public class PlayerUseTrigger : MonoBehaviour
{
    [SerializeField] private Player player;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.isStatic) return;
        if (!other.CompareTag(nameof(Usable))) return;
        foreach (var item in other.GetComponentsInParent<Component>())
        {
            if (item.GetType().IsSubclassOf(typeof(Usable)))
            {
                //print(item);
                player.useList.Add(other, (Usable)item);
                return;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        player.useList.Remove(other);
    }
}
