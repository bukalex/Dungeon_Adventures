using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private PlayerData playerData;
    
    [SerializeField]
    private TMP_Text coinCounter;
    
    [SerializeField]
    private healthBar HealthBar;

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
        //HealthBar.SetHealth(playerData.health);

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


}
