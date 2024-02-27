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
        depositButton.interactable = false;
        silverCopperSlider.interactable = false;
        goldInput.SetTextWithoutNotify((playerData.resources[Item.CoinType.GoldenCoin] + (int)value).ToString());
        silverInput.SetTextWithoutNotify((playerData.resources[Item.CoinType.SilverCoin] - (int)value * 100).ToString());
        silverInput1.text = silverInput.text;
    }

    public void OnSCSliderValueChanged(float value)
    {
        depositButton.interactable = false;
        goldSilverSlider.interactable = false;
        silverInput1.SetTextWithoutNotify((playerData.resources[Item.CoinType.SilverCoin] + (int)value).ToString());
        silverInput.text = silverInput1.text;
        copperInput.SetTextWithoutNotify((playerData.resources[Item.CoinType.CopperCoin] - (int)value * 100).ToString());
    }

    public void OnGInput(string text)
    {
        if (int.TryParse(text, out int result))
        {
            result = Mathf.Clamp(result, playerData.resources[Item.CoinType.GoldenCoin], playerData.resources[Item.CoinType.GoldenCoin] + (int)goldSilverSlider.maxValue);
            goldSilverSlider.SetValueWithoutNotify(result - playerData.resources[Item.CoinType.GoldenCoin]);
            OnGSSliderValueChanged(goldSilverSlider.value);
        }
    }

    public void OnSInput(string text)
    {
        if (int.TryParse(text, out int result))
        {
            result = Mathf.Clamp(result, 0, playerData.resources[Item.CoinType.SilverCoin]);
            goldSilverSlider.SetValueWithoutNotify((playerData.resources[Item.CoinType.SilverCoin] - result) / 100);
            OnGSSliderValueChanged(goldSilverSlider.value);
        }
    }
    public void OnS1Input(string text)
    {
        if (int.TryParse(text, out int result))
        {
            result = Mathf.Clamp(result, playerData.resources[Item.CoinType.SilverCoin], playerData.resources[Item.CoinType.SilverCoin] + (int)silverCopperSlider.maxValue);
            silverCopperSlider.SetValueWithoutNotify(result - playerData.resources[Item.CoinType.SilverCoin]);
            OnSCSliderValueChanged(silverCopperSlider.value);
        }
    }

    public void OnCInput(string text)
    {
        if (int.TryParse(text, out int result))
        {
            result = Mathf.Clamp(result, 0, playerData.resources[Item.CoinType.CopperCoin]);
            silverCopperSlider.SetValueWithoutNotify((playerData.resources[Item.CoinType.CopperCoin] - result) / 100);
            OnSCSliderValueChanged(silverCopperSlider.value);
        }
    }

    public void Convert()
    {
        playerData.resources[Item.CoinType.GoldenCoin] = int.Parse(goldInput.text);
        playerData.resources[Item.CoinType.SilverCoin] = int.Parse(silverInput.text);
        playerData.resources[Item.CoinType.CopperCoin] = int.Parse(copperInput.text);

        depositButton.interactable = InventoryManager.Instance.HasCoins();
        goldSilverSlider.interactable = true;
        goldSilverSlider.SetValueWithoutNotify(0);
        goldSilverSlider.maxValue = playerData.resources[Item.CoinType.SilverCoin] / 100;
        silverCopperSlider.interactable = true;
        silverCopperSlider.SetValueWithoutNotify(0);
        silverCopperSlider.maxValue = playerData.resources[Item.CoinType.CopperCoin] / 100;

        UpdateText();
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

        goldSilverSlider.maxValue = playerData.resources[Item.CoinType.SilverCoin] / 100;
        silverCopperSlider.maxValue = playerData.resources[Item.CoinType.CopperCoin] / 100;
    }
}
