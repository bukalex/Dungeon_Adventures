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
                playerData.resources[Item.CoinType.GoldenCoin] += 100;
                playerData.resources[Item.CoinType.SilverCoin] += 100;
                playerData.resources[Item.CoinType.CopperCoin] += 100;
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
        }
    }

    public void recieveText(string stringInput)
    {
        input.GetComponentInChildren<TMP_Text>().text = string.Empty;
        command = stringInput;
        chatLog.text += "Player:" + stringInput + "\n";

        if (command == "/give coins")
        {
            playerData.resources[Item.CoinType.GoldenCoin] += 100;
            playerData.resources[Item.CoinType.SilverCoin] += 100;
            playerData.resources[Item.CoinType.CopperCoin] += 100;
        }

        if (command == "/reset coins")
        {
            playerData.resources[Item.CoinType.GoldenCoin] = 0;
            playerData.resources[Item.CoinType.SilverCoin] = 0;
            playerData.resources[Item.CoinType.CopperCoin] = 0;
        }
    }

    public bool ChatIsActive()
    {
        return chatMenu.activeSelf;
    }
}
