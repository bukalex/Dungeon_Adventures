using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BankerWindow : MonoBehaviour
{
    [SerializeField]
    private PlayerData playerData;

    [SerializeField]
    private TMP_Dropdown dropdownLeft;
    [SerializeField]
    private TMP_Dropdown dropdownRight;
    [SerializeField]
    private TMP_InputField inputLeft;
    [SerializeField]
    private TMP_InputField inputRight;
    [SerializeField]
    private Slider slider;
    [SerializeField]
    private TMP_Text multiplier;
    [SerializeField]
    private Button depositButton;
    [SerializeField]
    private Button convertButton;

    private int rateMultiplier = 0;
    private int rate;
    private float time = 0;
    private bool goingUp = false;

    void Update()
    {
        time += Time.deltaTime;
    }

    private void OnEnable()
    {
        UIManager.Instance.InventorySlots.SetActive(true);
        UIManager.Instance.npcWindowActive = true;
        depositButton.interactable = InventoryManager.Instance.HasCoins();

        if (rateMultiplier == 0) rateMultiplier = UnityEngine.Random.Range(1, 11);
        if (time > 120)
        {
            rateMultiplier = UnityEngine.Random.Range(1, 11);
            time = 0;
        }
        goingUp = dropdownRight.value < dropdownLeft.value;
        rate = (int)(rateMultiplier * Mathf.Abs(dropdownRight.value - dropdownLeft.value) * Mathf.Pow(2, System.Convert.ToInt32(dropdownRight.value < dropdownLeft.value)));

        UpdateText();
    }

    private void OnDisable()
    {
        UIManager.Instance.InventorySlots.SetActive(false);
        UIManager.Instance.npcWindowActive = false;
    }

    public void OnLeftCoinChanged(int value)
    {
        if (value == dropdownRight.value)
        {
            dropdownRight.SetValueWithoutNotify((value + 1) % 3);
        }
        goingUp = dropdownRight.value < dropdownLeft.value;
        rate = (int)(rateMultiplier * Mathf.Abs(dropdownRight.value - dropdownLeft.value) * Mathf.Pow(2, System.Convert.ToInt32(dropdownRight.value < dropdownLeft.value)));

        UpdateText();
    }

    public void OnRightCoinChanged(int value)
    {
        if (value == dropdownLeft.value)
        {
            dropdownLeft.SetValueWithoutNotify((value + 1) % 3);
        }
        goingUp = dropdownRight.value < dropdownLeft.value;
        rate = (int)(rateMultiplier * Mathf.Abs(dropdownRight.value - dropdownLeft.value) * Mathf.Pow(2, System.Convert.ToInt32(dropdownRight.value < dropdownLeft.value)));

        UpdateText();
    }

    public void OnLeftInput(string value)
    {
        int result;
        if (goingUp)
        {
            result = (int)Mathf.Clamp(int.Parse(value), 
                playerData.resources[Enum.Parse<Item.CoinType>(Enum.GetName(typeof(Item.CoinType), dropdownLeft.value + 1))] - slider.maxValue * rate, 
                playerData.resources[Enum.Parse<Item.CoinType>(Enum.GetName(typeof(Item.CoinType), dropdownLeft.value + 1))]);
            result = playerData.resources[Enum.Parse<Item.CoinType>(Enum.GetName(typeof(Item.CoinType), dropdownLeft.value + 1))] - result;
            result /= rate;
        }
        else
        {
            result = playerData.resources[Enum.Parse<Item.CoinType>(Enum.GetName(typeof(Item.CoinType), dropdownLeft.value + 1))] - (int)Mathf.Clamp(int.Parse(value), 0, slider.maxValue);
        }
        slider.SetValueWithoutNotify(result);
        OnSliderChanged(result);
    }

    public void OnRightInput(string value)
    {
        int result;
        if (goingUp)
        {
            result = (int)Mathf.Clamp(int.Parse(value), 0, slider.maxValue);
        }
        else
        {
            result = (int)Mathf.Clamp(int.Parse(value), 0, slider.maxValue * rate);
        }
        slider.SetValueWithoutNotify(result);
        OnSliderChanged(result);
    }

    public void OnSliderChanged(float value)
    {
        convertButton.interactable = true;
        int leftValue = playerData.resources[Enum.Parse<Item.CoinType>(Enum.GetName(typeof(Item.CoinType), dropdownLeft.value + 1))];
        int rightValue = 0;
        if (goingUp)
        {
            leftValue -= (int)value * rate;
            rightValue += (int)value;
        }
        else
        {
            leftValue -= (int)value;
            rightValue += (int)value * rate;
        }
        inputLeft.SetTextWithoutNotify(leftValue.ToString());
        inputRight.SetTextWithoutNotify(rightValue.ToString());
    }

    public void Convert()
    {
        playerData.resources[Enum.Parse<Item.CoinType>(Enum.GetName(typeof(Item.CoinType), dropdownLeft.value + 1))] = int.Parse(inputLeft.text);
        playerData.resources[Enum.Parse<Item.CoinType>(Enum.GetName(typeof(Item.CoinType), dropdownRight.value + 1))] += int.Parse(inputRight.text);
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
        convertButton.interactable = false;
        int leftValue = playerData.resources[Enum.Parse<Item.CoinType>(Enum.GetName(typeof(Item.CoinType), dropdownLeft.value + 1))];
        inputLeft.SetTextWithoutNotify(leftValue.ToString());
        inputRight.SetTextWithoutNotify("0");

        slider.SetValueWithoutNotify(0);
        slider.interactable = true;
        if (goingUp)
        {
            multiplier.text = rate + " / 1";
            slider.maxValue = leftValue / rate;
            if (leftValue / rate == 0) slider.interactable = false;
        }
        else
        {
            multiplier.text = "1 / "+ rate;
            slider.maxValue = leftValue;
            if (leftValue == 0) slider.interactable = false;
        }
    }
}
