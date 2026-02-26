using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager instance;
    [Header("Menu & Death System")]
    [SerializeField] private CanvasGroup pauseMenuUI;
    [SerializeField] private CanvasGroup deathMenuUI;
    [SerializeField] private CanvasGroup inventoryPanel;
    [SerializeField] private GameObject preDeathPanel;
    [SerializeField] private Image[] inventoryCells;
    [SerializeField] private Image displayItem;

    public Sprite nothingSprite;
    public TMP_Text DeathMessage;

    [NonSerialized] public bool _isPaused = false;
    [NonSerialized] public bool _isInventoryOpen = false;

    [Header("TextManager")]
    public TMP_Text hudText;
    private float timer = 0f;
    private float timerMax = 5f;
    private bool timerWorking = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Update()
    {
        //
        // Text Manager
        //
        if (timerWorking)
        {
            if (timer >= timerMax)
            {
                timerWorking = false;
                timer = 0f;
                hudText.text = "";
                hudText.alpha = 1f;
                return;
            }
            timer += Time.deltaTime;
            hudText.alpha = 1 - (timer / timerMax);
        }
        //
        // Player Ui
        //
        if (MainMechanics.instance.player.alive)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (displayItem.gameObject.activeInHierarchy) UndisplayItemOnScreen();
                else if (_isPaused) ResumeGame();
                else if (_isInventoryOpen) CloseInventory();
                else PauseGame();
            }
            if (Input.GetKeyDown(KeyCode.Tab) && !_isPaused)
            {
                if (displayItem.gameObject.activeInHierarchy) UndisplayItemOnScreen();
                else if (_isInventoryOpen) CloseInventory();
                else OpenInventory();
            }
        }
        else
        {
            if (preDeathPanel != null)
            {
                preDeathPanel.SetActive(true);
            }
            Invoke(nameof(ShowDeathMenu), 5f);
        }
    }
    // Game events
    private void PauseGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        MainMechanics.instance.player.canMove = false;
        pauseMenuUI.alpha = 1;
        pauseMenuUI.interactable = true;
        pauseMenuUI.blocksRaycasts = true;
        _isPaused = true;
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        MainMechanics.instance.player.canMove = true;
        pauseMenuUI.alpha = 0;
        pauseMenuUI.interactable = false;
        pauseMenuUI.blocksRaycasts = false;
        Time.timeScale = 1f;
        _isPaused = false;
    }
    // Death menu
    private void ShowDeathMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        deathMenuUI.alpha = 1;
        deathMenuUI.interactable = true;
        deathMenuUI.blocksRaycasts = true;
        Time.timeScale = 0f;
    }
    
    // Inventory
    private void OpenInventory()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        MainMechanics.instance.player.canMove = false;
        inventoryPanel.alpha = 1;
        inventoryPanel.interactable = true;
        inventoryPanel.blocksRaycasts = true;
        Time.timeScale = 0f;
        _isInventoryOpen = true;
        for (int i = 0; i < MainMechanics.instance.player.inventory.Length; i++)
        {
            if (MainMechanics.instance.player.inventory[i] != null)
            {
                inventoryCells[i].sprite = MainMechanics.instance.player.inventory[i].icon;
            }
            else
            {
                inventoryCells[i].sprite = nothingSprite;
            }
        }
    }
    public void CloseInventory()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        MainMechanics.instance.player.canMove = true;
        inventoryPanel.alpha = 0;
        inventoryPanel.interactable = false;
        inventoryPanel.blocksRaycasts = false;
        Time.timeScale = 1f;
        _isInventoryOpen = false;
    }
    // Item on screen display
    public void DisplayItemOnScreen(Sprite displaySprite)
    {
        if (_isInventoryOpen) CloseInventory();
        displayItem.gameObject.SetActive(true);
        displayItem.sprite = displaySprite;
    }
    private void UndisplayItemOnScreen()
    {
        displayItem.gameObject.SetActive(false);
    }
    // Text display
    public void ShowText(string text)
    {
        timerWorking = true;
        timer = 0f;
        hudText.alpha = 1f;
        hudText.text = text;
    }
    public void HideText()
    {
        timerWorking = false;
        timer = 0f;
        hudText.alpha = 1f;
        hudText.text = "";
    }
    //Не удалять, а то всё короче
    public void QuitToMainMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
