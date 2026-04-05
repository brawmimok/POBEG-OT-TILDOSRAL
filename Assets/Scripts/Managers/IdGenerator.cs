using UnityEngine;

[CreateAssetMenu]
public class IdGenerator : ScriptableObject
{
    private static IdGenerator _instance;
    public static IdGenerator Instance
    {
        get
        {
            if (_instance == null)
                _instance = Resources.Load<IdGenerator>("GlobalIdGenerator");
            return _instance;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatic()
    {
        _instance = null;
    }

    public ulong lastId;
    public ulong GetNextId()
    {
        lastId++;
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
        return lastId;
    }
}