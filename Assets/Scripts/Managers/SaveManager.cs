using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[AttributeUsage(AttributeTargets.Field)]
public class SaveableAttribute : Attribute { }

public class SaveManager : MonoBehaviour
{
    private string savePath;
    private JsonSerializerSettings jsonSettings;

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "pepus_pure_save.19");

        jsonSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Converters = new JsonConverter[] { new Vector3Converter(), new QuaternionConverter() }
        };
    }

    public void SaveGame()
    {
        List<Dictionary<string, object>> allObjectsData = new();

        foreach (var target in FindObjectsByType<PepusBehaviour>(FindObjectsInactive.Include))
        {
            target.OnBeforeSave();
            Dictionary<string, object> objectDict = new Dictionary<string, object>();
            Type targetType = target.GetType();
            Type saveableAttType = typeof(SaveableAttribute);

            objectDict["_objectType"] = targetType.AssemblyQualifiedName;

            FieldInfo[] fields = targetType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
                if (field.IsDefined(saveableAttType))
                    objectDict[field.Name] = field.GetValue(target);

            if (objectDict.Count > 0)
                allObjectsData.Add(objectDict);
        }

        string json = JsonConvert.SerializeObject(allObjectsData, jsonSettings);
        File.WriteAllText(savePath, json);
    }

    public void LoadGame()
    {
        if (!File.Exists(savePath)) return;

        string json = File.ReadAllText(savePath);
        var allObjectsData = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(json, jsonSettings);

        foreach (var entity in FindObjectsByType<PepusBehaviour>(FindObjectsInactive.Include)) Destroy(entity.gameObject);

        foreach (var objectDict in allObjectsData)
        {
            string typeName = objectDict.TryGetValue("_objectType", out object typeValue) ? typeValue?.ToString() : null;

            if (string.IsNullOrEmpty(typeName)) continue;

            Type objectType = Type.GetType(typeName);

            if (objectType == null) continue;
            object fieldInfo = objectType.GetField("PrefabResourcePath",
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).GetValue(null);
            if (fieldInfo == null) continue;

            string prefabPath = fieldInfo as string;

            GameObject _go = Instantiate((GameObject)Resources.Load(prefabPath));
            PepusBehaviour pepusComponent = _go.GetComponent<PepusBehaviour>();

            if (pepusComponent == null)
            {
                Debug.LogError("В префабе нет PepusBehaviour!");
                continue;
            }

            FieldInfo[] fields = pepusComponent.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
                if (objectDict.TryGetValue(field.Name, out object rawValue))
                {
                    string rawJson = JsonConvert.SerializeObject(rawValue, jsonSettings);
                    object convertedValue = JsonConvert.DeserializeObject(rawJson, field.FieldType, jsonSettings);
                    field.SetValue(pepusComponent, convertedValue);
                }

            pepusComponent.OnAfterLoad();
        }
    }
}