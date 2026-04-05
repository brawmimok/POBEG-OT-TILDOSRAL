using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{
    [SerializeField] private AudioSource music;
    [SerializeField] private string sceneName;

    [NonSerialized] public static bool shouldPlaySoundInCredits = true;
    private void Start()
    {
        if (shouldPlaySoundInCredits) music.Play();
        else Destroy(music);
        shouldPlaySoundInCredits = true;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CreditsExit();
        }
    }
    public void CreditsExit()
    {
        Destroy(FindAnyObjectByType<AudioSource>());
        SceneManager.LoadScene(sceneName);
        Destroy(this);
    }
}