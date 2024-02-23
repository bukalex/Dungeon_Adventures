using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LootRandomizer : MonoBehaviour
{
    public GameObject InventoryItemPref;
    public Item[] items;

    private InventoryManager inventoryManager;

    // Start is called before the first frame update
    public void Update()
    {
        int randomN = Random.Range(0, 101);
        foreach (Item item in items)
        {

        }


       
    }

    public void spawnNewItem(Item item, InventorySlot slot)
    {
        GameObject newItemGo = Instantiate(InventoryItemPref, slot.transform);
        InventoryItem inventoryItem = newItemGo.GetComponent<InventoryItem>();
        inventoryItem.InitializeItem(item);
    }

}
