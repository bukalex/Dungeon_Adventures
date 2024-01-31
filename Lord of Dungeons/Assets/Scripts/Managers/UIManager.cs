using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    //[SerializeField]
    //private PlayerData playerData;
    //
    //[SerializeField]
    //private TMP_Text coinCounter;
    //
    //[SerializeField]
    //private healthBar HealthBar;

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
        else
        {
            Debug.Log("Second UIManager was destroyed");
            Destroy(gameObject);
        }
    }

    void Update()
    {
        //Update values
        //coinCounter.text = playerData.resources[ItemParameters.ResourceType.COIN].ToString();
        //HealthBar.SetHealth(playerData.health);

        //Open inventory
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Button E was pressed!");
            inventory.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            inventory.SetActive(false);
        }
    }


}
