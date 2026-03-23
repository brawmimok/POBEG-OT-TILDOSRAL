using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class PepusBehaviour : MonoBehaviour
{
    [SerializeField] private ulong _id;
    public ulong Id => _id;
    [NonSerialized] public SaveData saveData;
    private void OnValidate()
    {
        if (!Application.isPlaying && _id == 0
#if UNITY_EDITOR
            && !PrefabUtility.IsPartOfPrefabAsset(this)
#endif
            )
        {
            if (IdGenerator.Instance != null)
            {
                _id = IdGenerator.Instance.GetNextId();
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }
            else
            {
                throw new("IdGenerator не существует!");
            }
        }
    }

    protected virtual void Awake()
    {
        if (_id == 0 && IdGenerator.Instance != null)
        {
            _id = IdGenerator.Instance.GetNextId();
        }
        SaveManager.instance.behaviours.Add(Id, this);
    }
    [Serializable]
    public class SaveData
    {
        public ulong id;
        public bool active;
    }
    virtual public void OnSave()
    {
        saveData = new() { active = gameObject.activeInHierarchy, id = Id };
    }
    virtual public void OnLoad(SaveData saveData)
    {
        gameObject.SetActive(saveData.active);
    }
}