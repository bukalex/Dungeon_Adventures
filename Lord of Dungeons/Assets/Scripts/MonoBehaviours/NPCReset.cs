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

    private void OnDisable()
    {
        TemporaryTradingSystem.itemInStore = null;

        if (traderPurchaseMenu != null && traderSellMenu != null)
        {
            storeButton.interactable = false;
            sellButton.interactable = true;
            traderPurchaseMenu.SetActive(true);
            traderSellMenu.SetActive(false);
            UIManager.Instance.InventorySlots.SetActive(false);
        }
    }
}
