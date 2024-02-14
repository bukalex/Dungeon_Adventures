using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    public PlayerData playerData;
    [SerializeField]
    public TMP_Text stats;
    [SerializeField]
    private TMP_Text coinCounter;
    [SerializeField]
    public GameObject HealthBar;
    [SerializeField]
    public GameObject ManaBar;
    [SerializeField]
    public GameObject StaminaBar;
    [SerializeField]
    public GameObject internalInventory;
    [SerializeField]
    public GameObject inventory;
    [SerializeField]
    public GameObject toolbar;

    //Assign Storage from Store Menu
    [SerializeField]
    public GameObject storage;
    [SerializeField]
    public GameObject sellSlots;
    [SerializeField] 
    public GameObject chestInventory;
    [SerializeField]
    public GameObject[] itemHolders;
    [SerializeField]
    private GameObject sellMenu, storeMenu;

    public bool isPaused = false;

     public GameObject[] spawnedEnemies;
    public GameObject[] enemyHealthBars;
    [SerializeField] private Canvas enemyUI;

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        //HealthBar.SetMaxHealth(playerData.maxHealth);

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    void Update()
    {
        //Update values
        displayStats();
        InitializeBars();

        //Initializing all enemy health bars
        if(spawnedEnemies != null)
        {
            //spawnedEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            enemyHealthBars = GameObject.FindGameObjectsWithTag("EnemyHealthBar");

            for(int i = 0; i < spawnedEnemies.Length; i++)
            {

            }
            InitializeEnemiesHealthBar();
        }

        //Open inventory
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("Button TAB was pressed!");
            inventory.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            inventory.SetActive(false);
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

    public void displayStats()
    {
        string HPstats = "HP: " + playerData.maxHealth + "\n"; 
        string ManaStats = "Mana: " + playerData.maxMana.ToString() + "\n";
        string StaminaStats = "Stamina: " + playerData.maxStamina.ToString() + "\n";
        string DamageStats = "Damage: " + playerData.attack.ToString() + "\n";
        string DefenseStats = "Defense: " + playerData.defense.ToString() + "\n";
        string SpeedStats = "Speed: " + playerData.speed.ToString() + "\n";
        

        stats.text = HPstats + ManaStats + StaminaStats + DamageStats + DefenseStats + SpeedStats;
    }

    public void traderButtons()
    {
        sellMenu.SetActive(!sellMenu.activeSelf);
        storeMenu.SetActive(!storeMenu.activeSelf);
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
