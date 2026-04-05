using System;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.SceneManagement;

public class MainMechanics : MonoBehaviour
{
    public static MainMechanics instance;
    public Player player;
    public AudioSource sfxAudio;
    public AudioSource musicAudio;
    public AudioSource uiAudio;
    public Material[] editorOnlyMaterials;

    private void Awake()
    {
        instance = this;
        if (player == null)
        {
            player = FindAnyObjectByType<Player>();
            if (player == null)
                Debug.LogError("Моросишь блеать! Где игрок сцуко?");
        }
        foreach (var meshRenderer in FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include))
            if (editorOnlyMaterials.Contains(meshRenderer.sharedMaterial))
            {
                var a = meshRenderer.gameObject.GetComponent<ProBuilderMesh>();
                if (a != null ) Destroy(a);
                Destroy(meshRenderer);
            }
    }

    //Scene manager
    public void SetDontDestroyOnLoad(UnityEngine.Object obj)
    {
        DontDestroyOnLoad(obj);
    }
    public void LoadCreditsScene(bool shouldPlayMusic)
    {
        CreditsManager.shouldPlaySoundInCredits = shouldPlayMusic;
        SceneManager.LoadScene("Credits");
    }
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}