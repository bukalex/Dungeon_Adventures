using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TemporaryTradingSystem : MonoBehaviour
{
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

    public void SellItem(bool itemInSlot)
    {
    }
}
