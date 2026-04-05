using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Space]
    [Header("MainMenu Settings")]
    [Space]
    [SerializeField]
    private AudioSource soundWhenStartingScene;
    private static bool _soundWhenStartingSceneHasPlayed = false;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatic()
    {
        _soundWhenStartingSceneHasPlayed = false;
    }
    [Space]
    [SerializeField]
    private AudioSource soundWhenExitingGame;
    private bool _soundWhenExitingGameHasPlayed = false;

    [Space]
    [Header("LoadGame Settings")]
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private GameObject loadingPanel;

    [SerializeField] private string sceneToLoad = "Scene";
    [SerializeField] private string sceneToLoadIntro = "IntroScene";

    [Space]
    [Header("MainMenu Intro")]
    [SerializeField] private Toggle introToggle;

    private bool isLoading = false;

    private void Start()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;


        if (!_soundWhenStartingSceneHasPlayed)
        {
            soundWhenStartingScene.Play();
            _soundWhenStartingSceneHasPlayed = true;
        }
    }

    public void StartGame()
    {
        if (isLoading) return;
        isLoading = true;

        loadingPanel.SetActive(true);
        StartCoroutine(LoadGameScene(introToggle.isOn ? sceneToLoadIntro : sceneToLoad));
    }

    private IEnumerator LoadGameScene(string scene)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            loadingText.text = "LOADING - " + (int)(operation.progress * 100f) + " %";
            if (operation.progress >= 0.9f)
            {
                loadingText.text = "LOADING - 100 %";
                yield return new WaitForSeconds(1f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }


    public void Quit()
    {
        if (!_soundWhenExitingGameHasPlayed)
        {
            soundWhenExitingGame.Play();
            _soundWhenExitingGameHasPlayed = true;
            Invoke(nameof(ApplicationQuit), 3f);
        }
    }

    private void ApplicationQuit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}