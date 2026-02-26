using System.Collections.Generic;
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

    [SerializeField] private bool cusorIsLock = true;

    private void Awake()
    {
        if (cusorIsLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

        if (instance == null)
        {
            instance = this;
        }
        if (player == null)
        {
            player = FindAnyObjectByType<Player>();
            if (player == null)
            {
                Debug.LogError("Моросишь блеать! Где игрок сцуко?");
            }
        }
        foreach (var meshRenderer in FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (editorOnlyMaterials.Contains(meshRenderer.sharedMaterial))
            {
                Destroy(meshRenderer);
            }
        }
    }

    //Scene manager
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}