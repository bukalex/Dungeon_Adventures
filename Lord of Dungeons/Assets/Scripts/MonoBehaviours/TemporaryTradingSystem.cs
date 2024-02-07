using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class TemporaryTradingSystem : MonoBehaviour
{
    public InventorySlot sellSlot;
    public PlayerData playerData;
    public Item[] items2Pickup;
    public InventoryItem itemInStore;
    public TMP_Text priceDisplay;
    public TMP_Text itemName;
    public TMP_Text itemDescription;
    public int coinsFromSale;

    public GraphicRaycaster graphicRaycaster;
    public EventSystem eventSystem;
    private PointerEventData eventData;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            eventData = new PointerEventData(eventSystem);
            eventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            graphicRaycaster.Raycast(eventData, results);
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.GetComponent<InventoryItem>() != null)
                {
                    itemInStore = result.gameObject.GetComponent<InventoryItem>();
                    itemName.text = itemInStore.item.name;
                    itemDescription.text = itemInStore.item.description;
                    break;
                }
            }
        }
    }

    public void PickUpItem(int id)
    {

        if (items2Pickup[id].price <= playerData.resources[Item.MaterialType.Coin])
        {
            InventoryManager.Instance.AddItem(items2Pickup[id]);

            playerData.resources[Item.MaterialType.Coin] -= items2Pickup[id].price;
        }
    }

    public void sellItems()
    {
        estimateSale();
        InventoryItem[] ItemInSlot = new InventoryItem[InventoryManager.Instance.sellSlots.Length];

        for (int i = 0; i < InventoryManager.Instance.sellSlots.Length; i++)
        {
            foreach (InventorySlot sellSlot in InventoryManager.Instance.sellSlots)
            {
                ItemInSlot[i] = sellSlot.GetComponentInChildren<InventoryItem>();
                Destroy(ItemInSlot[i]);
            }
        }

        playerData.resources[Item.MaterialType.Coin] += coinsFromSale;
    }

    public void estimateSale()
    {
        InventorySlot slot = GetComponent<InventorySlot>();
        InventoryItem[] ItemInSlot = new InventoryItem[InventoryManager.Instance.sellSlots.Length];

        for (int i = 0; i < InventoryManager.Instance.sellSlots.Length; i++)
        {
            foreach(InventorySlot sellSlot in InventoryManager.Instance.sellSlots)
            {
                ItemInSlot[i] = sellSlot.GetComponentInChildren<InventoryItem>();
            }
        }

        for (int i = 0; i < ItemInSlot.Length; i++)
        {
            coinsFromSale = ItemInSlot[i].item.price;
        }

        priceDisplay.text = "You got " + coinsFromSale.ToString();
    }

    private bool isSlotEmpty()
    {

        InventorySlot slot = sellSlot;
        InventoryItem itemInSlot = sellSlot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot != null)
            return false;

        Debug.Log("Slot is empty");
        return true;
    }
}

