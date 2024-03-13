using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    public PlayerData playerData;
    [SerializeField]
    public TMP_Text stats, goldenCoinCounter, silverCoinCounter, copperCoinCounter;
    [SerializeField]
    public GameObject HealthBar, ManaBar, StaminaBar;
    [SerializeField]
    public GameObject inventory, toolbar, abilitybar, abilityInventory, equipment;
    [SerializeField] 
    public GameObject cheatChestUIs;
    [SerializeField]
    private Button abilityInventoryOpen, abilityInventoryClose;

    //UI to open on a button
    [SerializeField]
    public GameObject InventorySlots, EquipmentSection;

    //Assign Storage from Store Menu
    [SerializeField]
    public GameObject traderStorage;
    [SerializeField]
    public GameObject wizardStorage;
    [SerializeField]
    private GameObject itemHolderPrefab;
    [SerializeField]
    private GameObject teleportButtonPrefab;
    [SerializeField]
    public GameObject sellSlots;
    [SerializeField]
    public GameObject wizardSellSlots;
    [SerializeField]
    private GameObject traderSellMenu, traderStoreMenu, wizardSellMenu, wizardStoreMenu;
    [SerializeField]
    private Button traderSellButton, traderStoreButton, wizardSellButton, wizardStoreButton;
    [SerializeField]
    public GameObject traderWindow, wizardWindow, teleportWindow, bankerWindow, blacksmithWindow;
    [SerializeField]
    private GameObject teleportContent;
    [SerializeField]
    private Item[] traderItems;
    [SerializeField]
    private Item[] wizardItems;

    [Header("Chest UI Properties")]
    [SerializeField] private GameObject ChestUI;
    [SerializeField] public GameObject[] ChestUIs, Chests;
    [SerializeField] private Canvas playerCanvas;

    [Header("Escape Menu Properties")]
    [SerializeField] private GameObject escapeUI, escapeButtons, settingUI;
    [SerializeField] private Button resumeButton, settingButton, quitButton;
    [SerializeField] private Button[] changeButtons;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] public TMP_Text[] textKeys;
    [SerializeField] private TMP_Text[] barLabels;
    public KeyCode[] keyCodes;
    private int chosenIndex = -1;
    private Resolution[] resolutions;

    public bool isPaused = false;
    public bool npcWindowActive = false;

    public GameObject[] spawnedEnemies;
    public GameObject[] enemyHealthBars;
    public TMP_Text bossCounter, enemyCounter, levelCounter;

    [SerializeField] private Transform exitDirection;
    private Transform exit;
    private Vector3 screenDirection;

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            tag = "MainCanvas";
            DontDestroyOnLoad(this);

            InitializeNPCItems(traderItems, traderStorage);
            InitializeNPCItems(wizardItems, wizardStorage);
            InitializeKeyCodeSettings();
        }
        else if (tag != "MainCanvas")
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        options.Clear();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "X" + resolutions[i].height;
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
                currentResolutionIndex = i;
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    void Update()
    {
        //Initialize chests UI
        //ChestUIs = GameObject.FindGameObjectsWithTag("ChestUI");
        Cursor.visible = npcWindowActive || InventorySlots.activeSelf || escapeUI.activeSelf || CheatManager.Instance.ChatIsActive();

        //Update values
        displayInventoryUI();
        InitializeBars();

        //Initializing all enemy health bars
        if(spawnedEnemies != null)
        {
            enemyHealthBars = GameObject.FindGameObjectsWithTag("EnemyHealthBar");
            InitializeEnemiesHealthBar();
        }

        if (chosenIndex != -1 && Input.anyKeyDown)
        {
            string newText = "";
            KeyCode newCode = KeyCode.None;
            int existingIndex;
            if (Input.GetMouseButtonDown(0) && chosenIndex != 12)
            {
                newText = "LMB";
                newCode = KeyCode.Mouse0;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                newText = "RMB";
                newCode = KeyCode.Mouse1;
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && chosenIndex != 6)
            {
                newText = "ESC";
                newCode = KeyCode.Escape;
            }
            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                newText = "TAB";
                newCode = KeyCode.Tab;
            }
            else if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                newText = "SHIFT";
                newCode = KeyCode.LeftShift;
            }
            else if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                newText = "CTRL";
                newCode = KeyCode.LeftControl;
            }
            else if (!int.TryParse(Input.inputString, out int result) &&  Enum.TryParse(Input.inputString, true, out newCode))
            {
                newText = Input.inputString;
            }

            if (newCode != KeyCode.None)
            {
                existingIndex = Array.IndexOf(keyCodes, newCode);
                if (existingIndex != -1)
                {
                    textKeys[existingIndex].text = textKeys[chosenIndex].text;
                    keyCodes[existingIndex] = keyCodes[chosenIndex];
                }
                textKeys[chosenIndex].text = newText;
                keyCodes[chosenIndex] = newCode;
                chosenIndex = -1;
            }
        }
        barLabels[0].text = textKeys[8].text;
        barLabels[1].text = textKeys[9].text;
        barLabels[2].text = textKeys[10].text;
        barLabels[3].text = textKeys[11].text;

        abilityInventoryOpen.interactable = SceneManager.GetActiveScene().name == "HUB";
        abilityInventoryClose.interactable = SceneManager.GetActiveScene().name == "HUB";


        //Open inventory
        if (!npcWindowActive)
        {
            if (Input.GetKeyDown(keyCodes[13]))
            {
                Debug.Log("Button TAB was pressed!");
                if(InventorySlots.activeSelf == EquipmentSection.activeSelf)
                {
                    InventorySlots.SetActive(!InventorySlots.activeSelf);
                    EquipmentSection.SetActive(!EquipmentSection.activeSelf);
                }
                else
                {
                    InventorySlots.SetActive(!InventorySlots.activeSelf);
                    EquipmentSection.SetActive(EquipmentSection.activeSelf);
                }
            }
        }

        if (!npcWindowActive && Input.GetKeyDown(keyCodes[12]))
        {
            escapeUI.SetActive(!escapeUI.activeSelf);
            if (escapeUI.activeSelf)
                Time.timeScale = 0f;
            if (!escapeUI.activeSelf)
            {
                Time.timeScale = 1.0f;
                chosenIndex = -1;
            }
                

            resumeButton.onClick.AddListener(() => Resume());
            settingButton.onClick.AddListener(() => Setting());
            quitButton.onClick.AddListener(() => Quit());

        }   

    }

    private void FixedUpdate()
    {
        UpdateExitDirection();
    }

    //Escape Menu Functions
    #region
    public void Resume()
    {
        Time.timeScale = 1.0f;
        chosenIndex = -1;
        escapeUI.SetActive(false);
    }
    public void Setting() 
    { 
        settingUI.SetActive(!settingUI.activeSelf);
        if (settingUI.activeSelf)
            escapeButtons.transform.position = new Vector3(560f, 600f);
        else
            escapeButtons.transform.position = new Vector3(960f, 600f);
    }
    public void Quit()
    {
        Application.Quit();
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    public void SetResolution(int currentIndex)
    {
        Resolution resolution = resolutions[currentIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    private void ChangeKey(int index)
    {
        if (chosenIndex == -1)
        {
            chosenIndex = index;
        }
        else
        {
            chosenIndex = -1;
        }
    }
    #endregion

    public void displayInventoryUI()
    {
        //Display player stats
        string HPstats = "HP: " + playerData.maxHealth.ToString("F2") + "\n"; 
        string ManaStats = "Mana: " + playerData.maxMana.ToString("F2") + "\n";
        string StaminaStats = "Stamina: " + playerData.maxStamina.ToString("F2") + "\n";
        string SpeedStats = "Speed: " + playerData.speed.ToString("F2") + "\n";
        string AttackStats = "Attack: " + playerData.attack.ToString("F2") + "\n";
        string DefenseStats = "Defense: " + playerData.defense.ToString("F2") + "\n";
        string SpecialAttackStats = "Sp. attack: " + playerData.specialAttack.ToString("F2") + "\n";
        string SpecialDefenseStats = "Sp. defense: " + playerData.specialDefense.ToString("F2") + "\n";
        stats.text = HPstats + ManaStats + StaminaStats + SpeedStats + AttackStats + DefenseStats + SpecialAttackStats + SpecialDefenseStats;

        //Display coins amount
        goldenCoinCounter.text = playerData.resources[Item.CoinType.GoldenCoin].ToString();
        silverCoinCounter.text = playerData.resources[Item.CoinType.SilverCoin].ToString();
        copperCoinCounter.text = playerData.resources[Item.CoinType.CopperCoin].ToString();
    }

    public void traderButtons(int npcIndex)
    {
        switch (npcIndex)
        {
            case 0:
                traderSellButton.interactable = !traderSellButton.interactable;
                traderStoreButton.interactable = !traderStoreButton.interactable;

                traderSellMenu.SetActive(!traderSellMenu.activeSelf);
                traderStoreMenu.SetActive(!traderStoreMenu.activeSelf);

                InventorySlots.SetActive(traderSellMenu.activeSelf);
                break;

            case 1:
                wizardSellButton.interactable = !wizardSellButton.interactable;
                wizardStoreButton.interactable = !wizardStoreButton.interactable;

                wizardSellMenu.SetActive(!wizardSellMenu.activeSelf);
                wizardStoreMenu.SetActive(!wizardStoreMenu.activeSelf);

                InventorySlots.SetActive(wizardSellMenu.activeSelf);
                break;
        }
    }

    private void InitializeNPCItems(Item[] items, GameObject storage)
    {
        foreach (Item item in items)
        {
            Transform itemHolder = Instantiate(itemHolderPrefab, storage.transform).transform;
            InventoryItem inventoryItem = itemHolder.GetComponentInChildren<InventorySlot>().GetComponentInChildren<InventoryItem>();
            inventoryItem.item = item;
            inventoryItem.GetComponent<Image>().sprite = item.image;
            inventoryItem.transform.GetChild(0).gameObject.SetActive(false);

            itemHolder.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = item.GoldenCoin.ToString();
            itemHolder.GetChild(0).GetChild(3).GetComponent<TMP_Text>().text = item.SilverCoin.ToString();
            itemHolder.GetChild(0).GetChild(5).GetComponent<TMP_Text>().text = item.CopperCoin.ToString();

            inventoryItem.isLocked = true;
        }
    }

    public void InitializeTeleportWindow(int checkpoints, int period)
    {
        teleportContent.GetComponentInChildren<Button>().onClick.AddListener(delegate { GoToCheckpoint(1); });

        for (int i = 0; i < checkpoints; i++)
        {
            UpdateTeleportWindow(i + 1, period);
        }
    }

    public void UpdateTeleportWindow(int checkpoints, int period)
    {
        GameObject teleportButton = Instantiate(teleportButtonPrefab, teleportContent.transform);
        teleportButton.GetComponent<Button>().onClick.AddListener(delegate { GoToCheckpoint(CheckpointManager.Instance.checkpoints[UnityEngine.Random.Range(0, CheckpointManager.Instance.checkpoints.Count)]); });
        teleportButton.GetComponentInChildren<TMP_Text>().text = (checkpoints * period).ToString();
    }

    public void GoToCheckpoint(int checkpoint)
    {
        if (checkpoint != SceneManager.GetActiveScene().buildIndex)
        {
            CheckpointManager.Instance.levelsPassed = (checkpoint - 1) * 5;
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("PopUpUI"))
            {
                Destroy(obj);
            }
            teleportWindow.SetActive(false);
            npcWindowActive = false;
            bossCounter.text = "0";
            enemyCounter.text = "0";
            if (CheckpointManager.Instance.levelsPassed == 0) levelCounter.text = "HUB";
            else levelCounter.text = "Level " + CheckpointManager.Instance.levelsPassed.ToString();
            SceneManager.LoadScene(checkpoint);
        }
    }

    public void InitializeEnemiesHealthBar()
    {
        foreach (GameObject enemyHealthBar in enemyHealthBars)
        {
                EnemyController enemy = enemyHealthBar.transform.parent.GetComponentInParent<EnemyController>();
                
                enemyHealthBar.GetComponent<Slider>().value = enemy.enemyParameters.health;
                enemyHealthBar.GetComponent<Slider>().maxValue = enemy.enemyParameters.maxHealth;
        }
    }

    public void InitializeBars()
    {
        SetMaxBarValue(playerData.maxHealth, HealthBar);
        SetMaxBarValue(playerData.maxMana, ManaBar);
        SetMaxBarValue(playerData.maxStamina, StaminaBar);

        SetBarValue(playerData.health, HealthBar);
        SetBarValue(playerData.mana, ManaBar);
        SetBarValue(playerData.stamina, StaminaBar);
    }

    public void InitializeKeyCodeSettings()
    {
        for (int i = 0; i < changeButtons.Length; i++)
        {
            AssignIndex(i);
            KeyCode newCode = keyCodes[i];
            string newText = "";
            switch (newCode)
            {
                case KeyCode.Mouse0:
                    newText = "LMB";
                    break;
                case KeyCode.Mouse1:
                    newText = "RMB";
                    break;
                case KeyCode.Escape:
                    newText = "ESC";
                    break;
                case KeyCode.Tab:
                    newText = "TAB";
                    break;
                case KeyCode.LeftShift:
                    newText = "SHIFT";
                    break;
                case KeyCode.LeftControl:
                    newText = "CTRL";
                    break;
                default:
                    newText = Enum.GetName(typeof(KeyCode), newCode);
                    break;
            }
            textKeys[i].text = newText;
        }
    }

    private void AssignIndex(int index)
    {
        changeButtons[index].onClick.RemoveAllListeners();
        changeButtons[index].onClick.AddListener(delegate { ChangeKey(index); });
    }

    public void SetMaxBarValue(float health, GameObject Bar)
    {
        Bar.GetComponent<Slider>().value = health;
        Bar.GetComponent<Slider>().maxValue = health;
    }

    public void SetBarValue(float health, GameObject Bar)
    {
        Bar.GetComponent<Slider>().value = health;
    }

    public void ChangeSFXVolume(float value)
    {
        SoundManager.Instance.seMasterVolume = value;
    }

    public void ChangeMusicVolume(float value)
    {
        BGMSoundData data = SoundManager.Instance.bgmSoundDatas.Find(data => data.bgm == SoundManager.Instance.music);
        SoundManager.Instance.bgmMasterVolume = value;
        SoundManager.Instance.bgmAudioSource.volume = data.volume * SoundManager.Instance.bgmMasterVolume * SoundManager.Instance.masterVolume;
    }

    private void UpdateExitDirection()
    {
        if (exit == null) exit = GameObject.FindGameObjectWithTag("Finish").transform;
        screenDirection = Camera.main.WorldToScreenPoint(exit.position) - Camera.main.WorldToScreenPoint(playerData.position);
        float factor;
        float diagonalAngle = Mathf.Atan2(Screen.width / 2, Screen.height/2) * Mathf.Rad2Deg;
        float angle = Vector3.SignedAngle(Vector3.up, screenDirection, Vector3.forward);
        exitDirection.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        if (Mathf.Abs(angle) <= diagonalAngle || (180 - diagonalAngle) <= Mathf.Abs(angle))
        {
            factor = Mathf.Abs((Screen.height / 2 - 25) / Mathf.Cos(angle * Mathf.Deg2Rad)) / screenDirection.magnitude;
        }
        else
        {
            factor = Mathf.Abs((Screen.width / 2 - 25) / Mathf.Sin(angle * Mathf.Deg2Rad)) / screenDirection.magnitude;
        }
        if (factor >= 1) exitDirection.GetComponent<Image>().enabled = false;
        else exitDirection.GetComponent<Image>().enabled = true;
        exitDirection.position = Camera.main.WorldToScreenPoint(Vector3.Lerp(playerData.position, exit.position, factor));
    }
}
