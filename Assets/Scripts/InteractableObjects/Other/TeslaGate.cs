using UnityEngine;
using System.Collections;

public class TeslaGate : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField] private GameObject killZone;
    [SerializeField] private GameObject ElecTexture;
    
    [Space] [Header("Sounds")]
    [SerializeField] private AudioSource teslaSound;

    private Coroutine _teslaRoutine;
    private bool _playerInTrigger = false;

    private WaitForSeconds textureChangeSeconds = new(0.05f);
    private WaitForSeconds teslaInterval = new(1.5f);

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInTrigger = true;
                
            if (_teslaRoutine == null)
            {
                _teslaRoutine = StartCoroutine(TeslaLoop());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInTrigger = false;
        }
    }
    private IEnumerator TeslaLoop()
    {
        while (_playerInTrigger)
        {
            teslaSound.Play();
            killZone.SetActive(true);
            ElecTexture.SetActive(true);

            yield return textureChangeSeconds;
            ElecTexture.transform.Rotate(0f, 180f, 0f);
            yield return textureChangeSeconds;
            ElecTexture.transform.Rotate(0f, 0f, 0f);
            yield return textureChangeSeconds;
            ElecTexture.transform.Rotate(0f, 180f, 0f);
            yield return textureChangeSeconds;
            ElecTexture.transform.Rotate(0f, 0f, 0f);
            yield return textureChangeSeconds;

            ElecTexture.SetActive(false);
            killZone.SetActive(false);
            yield return teslaInterval;
        }
        
         killZone.SetActive(false);
         teslaSound.Stop();
        _teslaRoutine = null;
    }
}
