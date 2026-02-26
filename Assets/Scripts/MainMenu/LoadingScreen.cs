using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    public TextMeshProUGUI loadingText;

    private void Start()
    {
        StartCoroutine(LoadMainMenu());
    }

    IEnumerator LoadMainMenu()
    {
        var operation = SceneManager.LoadSceneAsync("MainMenu");
        operation.allowSceneActivation = false; // Ждём 100%

        while (operation.progress < 0.9f)
        {
            var percentage = Mathf.RoundToInt(operation.progress * 100);
            loadingText.text = "LOADING - " + percentage + " %";
            yield return null;
        }

        loadingText.text = "LOADING - 100 %";
        yield return new WaitForSeconds(1f); // Даем секунду, чтобы игрок видел 100%
        operation.allowSceneActivation = true; // Переход в главное меню}
    }
}