using UnityEngine;

public class OnlyEditorMonoBehaviour : MonoBehaviour
{
    private void Awake() => Destroy(Application.isEditor ? this : gameObject);
}