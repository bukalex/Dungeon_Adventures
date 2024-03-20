using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.InventoryElements;
public class test : MonoBehaviour
{
    public Item[] items2Pickup;

    public void PickUpItem(int id)
    {
        // InventoryManager.Instance.AddItem(items2Pickup[id]);
    }
}
