using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Events.DelayedUnityEvent;

public class Button : LockableUsable
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonSound;
    [SerializeField] private AudioClip warningSound;

    public DelayedUnityEvent buttonEvent;

    [SerializeField] private sbyte keyCardLevel;
    [SerializeField] private string reason;

    static private string keyCardInvalidLevelText = "Здесь нужна ключиковая карточка с охранным отверстием - отверстие {0} или выше уровня";
    static private string brokenButton = "Кнопка не выполняет следующие действия - {0}";

    private byte pressCount = 0;
    private float resetTime = 2f; // Через сколько секунд счётчик сбрасывается (с крыший)
    private float resetAudioTime = 0f; // Через сколько секунд счётчик сбрасывается audio ebanoe

    private float lastPressTime;
    private float lastPlayTime;

    protected override void Awake()
    {
        base.Awake();
        if (warningSound != null)
            resetAudioTime = warningSound.length;

    }
    override public void Use(Player player)
    {
        audioSource?.PlayOneShot(buttonSound);
        base.Use(player);

        if (player.keyCardLevel >= keyCardLevel && keyCardLevel >= 0)
        {
            buttonEvent.Invoke();
            return;
        }

        if (Time.time - lastPressTime > resetTime)
            pressCount = 0; // Сброс счётчика, если прошло слишком мало(наебал) времени

        pressCount++;
        lastPressTime = Time.time;
        if (pressCount > 5 && Time.time - lastPlayTime > resetAudioTime)
        {
            audioSource?.PlayOneShot(warningSound);
            lastPlayTime = Time.time;
            pressCount = 0; // Сбрасываем счётчик
        }

        GameUIManager.instance.ShowText(keyCardLevel <= -1 ? string.Format(brokenButton, reason) : string.Format(keyCardInvalidLevelText, keyCardLevel));
    }
    // Хуй хуй хуй
    public class ButtonSaveData : DelayedSaveData
    {
        public bool locked;
    }
    public override void OnSave()
    {
        saveData = new ButtonSaveData()
        {
            timeElapsed = buttonEvent.timeElapsed,
            locked = locked,

            active = gameObject.activeInHierarchy,
            id = Id
        };
    }
    public override void OnLoad(SaveData saveData)
    {
        base.OnLoad(saveData);
        ButtonSaveData saveData1 = (ButtonSaveData)saveData;
        locked = saveData1.locked;

        buttonEvent.timeElapsed = saveData1.timeElapsed;
        if (buttonEvent.timeElapsed > 0f)
            buttonEvent.Invoke();
    }
}