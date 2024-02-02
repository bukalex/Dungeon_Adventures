using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

public class TemporaryTradingSystem : MonoBehaviour
{
    public InventorySlot sellSlot;
    public PlayerData playerData;
    public Item[] items2Pickup;

    public void PickUpItem(int id)
    {

        if (items2Pickup[id].price <= playerData.resources[Item.MaterialType.Coin])
        {
            InventoryManager.instance.AddItem(items2Pickup[id]);

            playerData.resources[Item.MaterialType.Coin] -= items2Pickup[id].price;
        }
    }

    public void SellItem()
    {
        InventorySlot slot = sellSlot;
        InventoryItem itemInSlot = sellSlot.GetComponentInChildren<InventoryItem>();

        if(isSlotEmpty() == false)
        {
            playerData.resources[Item.MaterialType.Coin] += itemInSlot.item.price;
            Destroy(itemInSlot.gameObject);
        }
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

