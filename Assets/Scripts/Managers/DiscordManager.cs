using Discord;
using UnityEditor;
using UnityEngine;

public class DiscordManager
#if UNITY_EDITOR
#else
    : MonoBehaviour
#endif
{
#if UNITY_EDITOR
    private static long applicationID = 1463677657618321601;
    //private static string largeImage = "icon1";
    //private static string largeText = "ПОБЕГ ОТ ДИЛЬДОСРАЛА || INDEV";
    private static string details = "СБЕГИТЕ ИЗ ПОДВАЛА СУМАСШЕДШЕГО РАЗРАБОТЧИКА POD!";
#else
    private static long applicationID = 1379887543201431693;
    //private static string largeImage = "icon";
    //private static string largeText = "ПОБЕГ ОТ ДИЛЬДОСРАЛА";
    private static string details = "СБЕГИТЕ ИЗ ПОДВАЛА СУМАСШЕДШЕГО ЮТУБЕРА ПО SCP:SL! ЧТОБЫ ПОКИНУТЬ ЭТО АДСКОЕ МЕСТО, ВАМ ПРИДЁТСЯ ПОЖЕРТВОВАТЬ ВСЕМ, ЧТО У ВАС ЕСТЬ. ВАМ ПРЕДСТОИТ БИТЬ ФАНФРОНАЛОВ, УВИДЕТЬ 18 ТРУПОВ И ПОСТАРАТЬСЯ НЕ СТАТЬ 19-М. ВАМ ПРИДЁТСЯ ГРАБИТЬ КАРАВАНЫ И МНОГОЕ ДРУГОЕ, ЧТО ЕСТЬ В ЭТОЙ ИГРЕ. ДА ВЫ ХОТЬ ЗУБ ВЫРВИТЕ У ДЕДУШКИ АЛЕКСА КРИВОЗУБОГО! ИГРАЙТЕ САМИ, ИГРАЙТЕ С НАМИ!";
#endif

    public static Discord.Discord discord;

    private static Activity activity;
    private static ActivityManager activityManager;
#if UNITY_EDITOR
    [InitializeOnLoadMethod]
#else
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
    static private void Initialize()
    {
        if (discord == null)
        {
            discord = new Discord.Discord(applicationID, (ulong)CreateFlags.Default);
            activityManager = discord.GetActivityManager();
            activity = new Activity
            {
                Details = details,
                //Assets =
                //{
                //LargeText = largeText
                //},
                Timestamps =
                {
                    Start = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                }
            };
            UpdateStatus();
#if UNITY_EDITOR
            EditorApplication.update += UpdateState;
            EditorApplication.quitting += OnApplicationQuit;
#else
            GameObject go = new GameObject("DiscordManager");
            go.AddComponent<DiscordManager>();
            DontDestroyOnLoad(go);
#endif
        }
    }
#if UNITY_EDITOR
    private static void OnApplicationQuit()
    {
        discord?.Dispose();
    }
#else
    private void OnApplicationQuit()
    {
        discord?.Dispose();
    }
    private void Update()
    {
    UpdateState();
    }
#endif
#if UNITY_EDITOR
    static
#endif
    private void UpdateState()
    {
        try
        {
            discord?.RunCallbacks();
        }
        catch (ResultException ex)
        {
            if (ex.Result == Result.NotRunning)
            {
#if UNITY_EDITOR
                EditorApplication.update -= UpdateState;

#else
                Destroy(this.gameObject);
#endif
            }
        }
    }

    private static void UpdateStatus()
    {
        try
        {
            activityManager.UpdateActivity(activity, (res) =>
            {
                if (res != Result.Ok)
                {
                    Debug.LogWarning("Не удалось подключиться к Discord: " + res);
                }
            });
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Ошибка при обновлении статуса Discord: " + ex.Message);
        }
    }

}