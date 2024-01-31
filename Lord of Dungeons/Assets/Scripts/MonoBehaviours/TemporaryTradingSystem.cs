using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryTradingSystem : MonoBehaviour
{
    public Item[] items2Pickup;

    public void PickUpItem(int id)
    {
        InventoryManager.instance.AddItem(items2Pickup[id]);
    }

    
}
