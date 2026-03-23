using UnityEngine.SceneManagement;
using UnityEngine;

public class StartWarning : MonoBehaviour
{
    [SerializeField] private string sceneName;
    public void StartMainGame() => SceneManager.LoadScene(sceneName);
}
