using System.Linq;
using UnityEngine;
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
        if (instance == null)
            instance = this;
        if (player == null)
        {
            player = FindAnyObjectByType<Player>();
            if (player == null)
                Debug.LogError("Моросишь блеать! Где игрок сцуко?");
        }
        foreach (var meshRenderer in FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (editorOnlyMaterials.Contains(meshRenderer.sharedMaterial))
                Destroy(meshRenderer);
    }

    //Scene manager
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}