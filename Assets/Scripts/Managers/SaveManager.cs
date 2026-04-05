using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class SaveEntry
{
    public string typeName;
    public string jsonData;
}
[Serializable]
public class SaveDataFile
{
    public SaveDataFile()
    {
        saveDatas = new();
    }
    public List<SaveEntry> saveDatas;
}
public class SaveManager : MonoBehaviour
{
    static bool isLoad = false;
    static public SaveManager instance;
    static public string saveName = string.Empty;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatic()
    {
        isLoad = false;
        saveName = string.Empty;
    }

    public Dictionary<ulong, PepusBehaviour> behaviours = new();

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        if (isLoad)
        {
            isLoad = false;
            LoadSave();
        }
    }
    public void LoadSave()
    {
        if (!Directory.Exists(Application.dataPath + "/Save")) Directory.CreateDirectory(Application.dataPath + "/Save");
        string path = Application.dataPath + "/Save/" + (saveName == string.Empty ? "_" : saveName) + ".json";
        string path1 = Application.dataPath + "/Save/" + (saveName == string.Empty ? "main_" : "main_" + saveName) + ".json";

        SaveMainData mainData = JsonUtility.FromJson<SaveMainData>(File.ReadAllText(path1));

        if (mainData.sceneName == SceneManager.GetActiveScene().name)
        {
            var plcoll = MainMechanics.instance.player.GetComponent<Collider>();
            SaveDataFile SaveDataFile = JsonUtility.FromJson<SaveDataFile>(File.ReadAllText(path));

            plcoll.enabled = false;

            foreach (var item in SaveDataFile.saveDatas)
            {
                var type = Type.GetType(item.typeName);
                var data = (PepusBehaviour.SaveData)JsonUtility.FromJson(item.jsonData, type);
                if (behaviours.TryGetValue(data.id, out var val))
                {
                    val.StopAllCoroutines();
                    val.OnLoad(data);
                }
            }
            plcoll.enabled = true;
        }
        else
        {
            isLoad = true;
            SceneManager.LoadScene(saveName);
        }
    }
    public void Save()
    {
        SaveDataFile SaveDataFile = new();

        if (!Directory.Exists(Application.dataPath + "/Save")) Directory.CreateDirectory(Application.dataPath + "/Save");
        string path = Application.dataPath + "/Save/" + (saveName == string.Empty ? "_" : saveName) + ".json";
        string path1 = Application.dataPath + "/Save/" + (saveName == string.Empty ? "main_" : "main_" + saveName) + ".json";

        SaveMainData mainData = new()
        {
            sceneName = SceneManager.GetActiveScene().name
        };
        File.WriteAllText(path1, JsonUtility.ToJson(mainData));


        foreach (var item in behaviours)
        {
            item.Value.OnSave();

            var entry = new SaveEntry
            {
                typeName = item.Value.saveData.GetType().AssemblyQualifiedName,
                jsonData = JsonUtility.ToJson(item.Value.saveData)
            };
            SaveDataFile.saveDatas.Add(entry);
        }

        File.WriteAllText(path, JsonUtility.ToJson(SaveDataFile));
    }
    private class SaveMainData
    {
        public string sceneName;
    }
}