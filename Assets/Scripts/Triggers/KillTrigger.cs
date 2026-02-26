using UnityEngine;

public class KillZone : MonoBehaviour
{
    [Header("Kill Zone Settings")]
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private string deathReason;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var playerScript = other.GetComponent<Player>();
        if (playerScript.alive)
        {
            playerScript.Death(deathReason);
            MainMechanics.instance.sfxAudio.PlayOneShot(deathSound);
        }
    }
}