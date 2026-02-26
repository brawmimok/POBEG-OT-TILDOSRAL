using UnityEngine;

public class OnlyEditorMonoBehaviour : MonoBehaviour
{
    private void Awake()
    {
#if UNITY_EDITOR
        Destroy(this);
#else
        Destroy(gameObject);
#endif
    }
}