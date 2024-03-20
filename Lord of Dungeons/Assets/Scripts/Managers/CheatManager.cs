using Assets.Scripts.Enums.ItemType;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CheatManager : MonoBehaviour
{
    [SerializeField]
    private PlayerData playerData;
    [SerializeField]
    private GameObject chatMenu;
    [SerializeField]
    private TMP_Text chatLog;
    [SerializeField]
    private TMP_InputField input;
    [SerializeField]
    private string command;

    public static CheatManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Update()
    {
        //chatLog.text = input.GetComponentInChildren<TMP_Text>().text;
        if (Input.GetKey(KeyCode.P))
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                playerData.resources[CoinType.GoldenCoin] += 100;
                playerData.resources[CoinType.SilverCoin] += 100;
                playerData.resources[CoinType.CopperCoin] += 100;
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                playerData.attack += 100;
                playerData.specialAttack += 100;
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                playerData.defense += 100;
                playerData.specialDefense += 100;
            }
        }
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            chatMenu.SetActive(!chatMenu.activeSelf);
            if(chatMenu.activeSelf)
                Time.timeScale = 0f;
            else if(!chatMenu.activeSelf) 
                Time.timeScale = 1f;
        }
    }

    public void recieveText(string stringInput)
    {
        input.GetComponentInChildren<TMP_Text>().text = string.Empty;
        command = stringInput;
        chatLog.text += "Player:" + stringInput + "\n";

        if (command == "/give coins")
        {
            playerData.resources[CoinType.GoldenCoin] += 100;
            playerData.resources[CoinType.SilverCoin] += 100;
            playerData.resources[CoinType.CopperCoin] += 100;
        }

        if (command == "/reset coins")
        {
            playerData.resources[CoinType.GoldenCoin] = 0;
            playerData.resources[CoinType.SilverCoin] = 0;
            playerData.resources[CoinType.CopperCoin] = 0;
        }

        if (command == "/immortality on")
        {
            playerData.health = 999999999.9f;
        }

        if (command == "/immortality off")
        {
            playerData.health = 100.0f;
        }

        if (command == "/high-speed on")
        {
            playerData.speed = 10f;
        }

        if (command == "/high-speed off")
        {
            playerData.speed = 3f;
        }

        if(command == "/god")
        {
            playerData.speed = 12.5f;
            playerData.attack = 12500.0f;
            playerData.health = 99999999999f;
        }
    }

    public bool ChatIsActive()
    {
        return chatMenu.activeSelf;
    }
}
