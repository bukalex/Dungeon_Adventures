using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BankerWindow : MonoBehaviour
{
    [SerializeField]
    private PlayerData playerData;

    [SerializeField]
    private TMP_InputField goldInput;
    [SerializeField]
    private TMP_InputField silverInput;
    [SerializeField]
    private TMP_InputField silverInput1;
    [SerializeField]
    private TMP_InputField copperInput;

    [SerializeField]
    private Slider goldSilverSlider;
    [SerializeField]
    private Slider silverCopperSlider;

    [SerializeField]
    private Button depositButton;

    private void OnEnable()
    {
        UIManager.Instance.InventorySlots.SetActive(true);
        UIManager.Instance.npcWindowActive = true;
        depositButton.interactable = InventoryManager.Instance.HasCoins();
        UpdateText();
    }

    private void OnDisable()
    {
        UIManager.Instance.InventorySlots.SetActive(false);
        UIManager.Instance.npcWindowActive = false;
    }

    public void OnGSSliderValueChanged(float value)
    {
        int result = (int)((playerData.resources[Item.CoinType.GoldenCoin] * 100 + playerData.resources[Item.CoinType.SilverCoin]) * (1 - value));
        playerData.resources[Item.CoinType.GoldenCoin] += (playerData.resources[Item.CoinType.SilverCoin] - result) / 100;
        playerData.resources[Item.CoinType.SilverCoin] = result + (playerData.resources[Item.CoinType.SilverCoin] - result) % 100;

        UpdateText();
    }

    public void OnSCSliderValueChanged(float value)
    {
        int result = (int)((playerData.resources[Item.CoinType.SilverCoin] * 100 + playerData.resources[Item.CoinType.CopperCoin]) * (1 - value));
        playerData.resources[Item.CoinType.SilverCoin] += (playerData.resources[Item.CoinType.CopperCoin] - result) / 100;
        playerData.resources[Item.CoinType.CopperCoin] = result + (playerData.resources[Item.CoinType.CopperCoin] - result) % 100;

        UpdateText();
    }

    public void OnGInput(string text)
    {
        if (int.TryParse(text, out int result))
        {
            result = Mathf.Clamp(result, 0, playerData.resources[Item.CoinType.GoldenCoin] + playerData.resources[Item.CoinType.SilverCoin] / 100);
            playerData.resources[Item.CoinType.SilverCoin] += (playerData.resources[Item.CoinType.GoldenCoin] - result) * 100;
            playerData.resources[Item.CoinType.GoldenCoin] = result;

            UpdateText();
        }
    }

    public void OnSInput(string text)
    {
        if (int.TryParse(text, out int result))
        {
            result = Mathf.Clamp(result, 0, playerData.resources[Item.CoinType.GoldenCoin] * 100 + playerData.resources[Item.CoinType.SilverCoin]);
            playerData.resources[Item.CoinType.GoldenCoin] += (playerData.resources[Item.CoinType.SilverCoin] - result) / 100;
            playerData.resources[Item.CoinType.SilverCoin] = result + (playerData.resources[Item.CoinType.SilverCoin] - result) % 100;

            UpdateText();
        }
    }
    public void OnS1Input(string text)
    {
        if (int.TryParse(text, out int result))
        {
            result = Mathf.Clamp(result, 0, playerData.resources[Item.CoinType.SilverCoin] + playerData.resources[Item.CoinType.CopperCoin] / 100);
            playerData.resources[Item.CoinType.CopperCoin] += (playerData.resources[Item.CoinType.SilverCoin] - result) * 100;
            playerData.resources[Item.CoinType.SilverCoin] = result;

            UpdateText();
        }
    }

    public void OnCInput(string text)
    {
        if (int.TryParse(text, out int result))
        {
            result = Mathf.Clamp(result, 0, playerData.resources[Item.CoinType.SilverCoin] * 100 + playerData.resources[Item.CoinType.CopperCoin]);
            playerData.resources[Item.CoinType.SilverCoin] += (playerData.resources[Item.CoinType.CopperCoin] - result) / 100;
            playerData.resources[Item.CoinType.CopperCoin] = result + (playerData.resources[Item.CoinType.CopperCoin] - result) % 100;

            UpdateText();
        }
    }

    public void Deposit()
    {
        foreach (InventorySlot slot in InventoryManager.Instance.toolBar)
        {
            InventoryItem inventoryItem = slot.GetComponentInChildren<InventoryItem>();
            if (inventoryItem != null && inventoryItem.item.itemType == Item.ItemType.Coin)
            {
                playerData.resources[inventoryItem.item.materialType] += inventoryItem.count;
                Destroy(inventoryItem.gameObject);
            }
        }

        foreach (InventorySlot slot in InventoryManager.Instance.internalInventorySlots)
        {
            InventoryItem inventoryItem = slot.GetComponentInChildren<InventoryItem>();
            if (inventoryItem != null && inventoryItem.item.itemType == Item.ItemType.Coin)
            {
                playerData.resources[inventoryItem.item.materialType] += inventoryItem.count;
                Destroy(inventoryItem.gameObject);
            }
        }

        depositButton.interactable = false;
        UpdateText();
    }

    private void UpdateText()
    {
        goldInput.text = playerData.resources[Item.CoinType.GoldenCoin].ToString();
        silverInput.text = playerData.resources[Item.CoinType.SilverCoin].ToString();
        silverInput1.text = playerData.resources[Item.CoinType.SilverCoin].ToString();
        copperInput.text = playerData.resources[Item.CoinType.CopperCoin].ToString();

        if (playerData.resources[Item.CoinType.GoldenCoin] + playerData.resources[Item.CoinType.SilverCoin] != 0)
        {
            goldSilverSlider.SetValueWithoutNotify(playerData.resources[Item.CoinType.GoldenCoin] * 100.0f / (playerData.resources[Item.CoinType.GoldenCoin] * 100.0f + playerData.resources[Item.CoinType.SilverCoin]));
        }
        else
        {
            goldSilverSlider.SetValueWithoutNotify(0);
        }

        if (playerData.resources[Item.CoinType.SilverCoin] + playerData.resources[Item.CoinType.CopperCoin] != 0)
        {
            silverCopperSlider.SetValueWithoutNotify(playerData.resources[Item.CoinType.SilverCoin] * 100.0f / (playerData.resources[Item.CoinType.SilverCoin] * 100.0f + playerData.resources[Item.CoinType.CopperCoin]));
        }
        else
        {
            silverCopperSlider.SetValueWithoutNotify(0);
        }
    }
}
