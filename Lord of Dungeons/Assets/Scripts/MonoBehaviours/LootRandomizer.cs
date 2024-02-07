using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootRandomizer : MonoBehaviour
{
    public GameObject InventoryItemPref;
    public Item[] items;
    // Start is called before the first frame update
    void Start()
    {
        int ranK = Random.Range(0, 3);
        int ranM = Random.Range(0, 5);

        for (int i = 0; i < InventoryManager.Instance.chestSlots.Length; i++)
        {
            if (ranK % ranM == 0 || ranK % ranM == 2)
            {
                spawnNewItem(items[ranK], InventoryManager.Instance.chestSlots[i]);
            }
        }
    }

    public void spawnNewItem(Item item, InventorySlot slot)
    {
        GameObject newItemGo = Instantiate(InventoryItemPref, slot.transform);
        InventoryItem inventoryItem = newItemGo.GetComponent<InventoryItem>();
        inventoryItem.InitializeItem(item);
    }

}
