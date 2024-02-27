using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    public PlayerData playerData;
    [SerializeField]
    public TMP_Text stats, goldenCoinCounter, silverCoinCounter, copperCoinCounter;
    [SerializeField]
    public GameObject HealthBar, ManaBar, StaminaBar;
    [SerializeField]
    public GameObject inventory, toolbar, abilitybar;
    [SerializeField] 
    public GameObject cheatChestUIs;


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
    private GameObject teleportWindow;
    [SerializeField]
    private GameObject teleportContent;
    [SerializeField]
    private ItemParam[] traderItems;
    [SerializeField]
    private ItemParam[] wizardItems;

    //Chest UI
    [SerializeField] private GameObject ChestUI;
    [SerializeField] private GameObject[] ChestUIs, Chests;
    [SerializeField] private Canvas playerCanvas;

    public bool isPaused = false;
    public bool npcWindowActive = false;

    public GameObject[] spawnedEnemies;
    public GameObject[] enemyHealthBars;

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);

            InitializeNPCItems(traderItems, traderStorage);
            InitializeNPCItems(wizardItems, wizardStorage);
        }
    }

    void Update()
    {
        //Initialize chests UI
        //ChestUIs = GameObject.FindGameObjectsWithTag("ChestUI");
        
        
        //Update values
        displayInventoryUI();
        InitializeBars();

        //Initializing all enemy health bars
        if(spawnedEnemies != null)
        {
            enemyHealthBars = GameObject.FindGameObjectsWithTag("EnemyHealthBar");
            InitializeEnemiesHealthBar();
        }




        //Open inventory
        if (!npcWindowActive)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Debug.Log("Button TAB was pressed!");
                InventorySlots.SetActive(true);
                EquipmentSection.SetActive(true);
            }
            else if (Input.GetKeyUp(KeyCode.Tab))
            {
                InventorySlots.SetActive(false);
                EquipmentSection.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Time.timeScale = 1;
            }
            else
            {
                Time.timeScale = 0;
            }

        }
    }

    public void displayInventoryUI()
    {
        //Display player stats
        string HPstats = "HP: " + playerData.maxHealth + "\n"; 
        string ManaStats = "Mana: " + playerData.maxMana.ToString() + "\n";
        string StaminaStats = "Stamina: " + playerData.maxStamina.ToString() + "\n";
        string DamageStats = "Damage: " + playerData.attack.ToString() + "\n";
        string DefenseStats = "Defense: " + playerData.defense.ToString() + "\n";
        string SpeedStats = "Speed: " + playerData.speed.ToString() + "\n";
        stats.text = HPstats + ManaStats + StaminaStats + DamageStats + DefenseStats + SpeedStats;

        //Display coins amount
        goldenCoinCounter.text = playerData.resources[ItemParam.CoinType.GoldenCoin].ToString();
        silverCoinCounter.text = playerData.resources[ItemParam.CoinType.SilverCoin].ToString();
        copperCoinCounter.text = playerData.resources[ItemParam.CoinType.CopperCoin].ToString();
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

    private void InitializeNPCItems(ItemParam[] items, GameObject storage)
    {
        foreach (ItemParam item in items)
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
        teleportContent.GetComponentInChildren<Button>().onClick.AddListener(delegate { GoToCheckpoint(0); });

        for (int i = 0; i < checkpoints; i++)
        {
            UpdateTeleportWindow(i + 1, period);
        }
    }

    public void UpdateTeleportWindow(int checkpoints, int period)
    {
        GameObject teleportButton = Instantiate(teleportButtonPrefab, teleportContent.transform);
        teleportButton.GetComponent<Button>().onClick.AddListener(delegate { GoToCheckpoint(checkpoints * period); });
        teleportButton.GetComponentInChildren<TMP_Text>().text = (checkpoints * period).ToString();
    }

    public void GoToCheckpoint(int checkpoint)
    {
        if (checkpoint != SceneManager.GetActiveScene().buildIndex)
        {
            teleportWindow.SetActive(false);
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

    public void SetMaxBarValue(float health, GameObject Bar)
    {
        Bar.GetComponent<Slider>().value = health;
        Bar.GetComponent<Slider>().maxValue = health;
    }

    public void SetBarValue(float health, GameObject Bar)
    {
        Bar.GetComponent<Slider>().value = health;
    }

}
