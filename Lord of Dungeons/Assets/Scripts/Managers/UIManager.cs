using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    public PlayerData playerData;

    [SerializeField]
    public TMP_Text stats;

    //[SerializeField]
    //private TMP_Text coinCounter;
    //
    //[SerializeField]
    //public Slider HealthBar;
    //
    //[SerializeField]
    //public Slider ManaBar;
    //
    //[SerializeField]
    //public Slider StaminaBar;

    [SerializeField]
    public GameObject inventory;

    public static UIManager Instance { get; private set; }

    public enum UIType { INVENTORY, HEALTHBAR}

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
        ////Update values
        //coinCounter.text = playerData.resources[Item.MaterialType.Coin].ToString();
        displayStats();
        //SetMaxBarValue(playerData.maxHealth, HealthBar);

        //SetBarValue(playerData.health, HealthBar);


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

    public void SetMaxBarValue(float health, Slider slider)
    {
        Debug.Log(health, slider);
        slider.value = health;
        slider.maxValue = health;
    }

    public void SetBarValue(float health, Slider slider)
    {
        slider.value = health;
    }

}
