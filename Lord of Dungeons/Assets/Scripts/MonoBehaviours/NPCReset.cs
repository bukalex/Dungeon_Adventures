using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCReset : MonoBehaviour
{
    [SerializeField]
    private GameObject traderPurchaseMenu;
    [SerializeField]
    private GameObject traderSellMenu;
    [SerializeField]
    private Button sellButton;
    [SerializeField]
    private Button storeButton;

    [SerializeField]
    private Transform content;

    private void OnDisable()
    {
        if (traderPurchaseMenu != null && traderSellMenu != null)
        {
            storeButton.interactable = false;
            sellButton.interactable = true;
            traderPurchaseMenu.SetActive(true);
            traderSellMenu.SetActive(false);
            UIManager.Instance.InventorySlots.SetActive(false);
        }
    }

    public void HideItems(List<string> exceptions)
    {
        foreach (InventorySlot slot in content.GetComponentsInChildren<InventorySlot>())
        {
            if (!exceptions.Contains(slot.GetComponentInChildren<InventoryItem>().item.name)) slot.transform.parent.gameObject.SetActive(false);
        }
    }

    public void ShowItems()
    {
        foreach (InventorySlot slot in content.GetComponentsInChildren<InventorySlot>())
        {
            slot.transform.parent.gameObject.SetActive(true);
        }
    }
}
