using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LootRandomizer : MonoBehaviour
{
    public GameObject InventoryItemPref;
    [Header("Loot Tables")]
    public Item[] totalItems;
    public Item[] materialLootTable;
    public Item[] weaponLootTable;
    public Item[] consumableLootTable;
    public Item[] uniqueLootTable;
    // You can fill out these loot tables manually so that only specific drops are there for specific enemies. check inventory manager for template loot tables
    public List<Item> randomLootTable = new List<Item>();
    public List<Item> randomMaterialLootTable = new List<Item>();
    public List<Item> randomWeaponLootTable = new List<Item>();
    public List<Item> randomConsumableLootTable = new List<Item>();
    public List<Item> randomUniqueLootTable = new List<Item>();
    // These are filled via the function below, this is what the enemy/chest will drop. no need to manually go through these.

    // Start is called before the first frame update
    public void Start()
    {
        // Creates a the droppable items for each individual loot table should work well for chests no idea how it will work for enemies. 
        // Call upon only one of these if you want a specific kind of loot table
        foreach (var item in totalItems)
            FillRandomLootTable(item, randomWeaponLootTable);

        foreach (var item in materialLootTable)
            FillRandomLootTable(item, randomMaterialLootTable);

        foreach (var item in weaponLootTable)
            FillRandomLootTable(item, randomWeaponLootTable);

        foreach (var item in consumableLootTable)
            FillRandomLootTable(item, randomConsumableLootTable);

        foreach (var item in uniqueLootTable)
            FillRandomLootTable(item, randomUniqueLootTable);

    }

    public bool FillRandomLootTable(Item item, List<Item> lootTable)
    {
        
        int randomNum = Random.Range(0, 101);


        if (randomNum <= item.rarity)
        {
            lootTable.Add(item);

            Debug.Log("Rolled: " + randomNum + ". And added: " + item + ". which has a rarity of: " + item.rarity);
        }
        else
        {
            Debug.Log("Rolled: " + randomNum + ". And thus could not add:" + item + ". which has a rarity of: " + item.rarity);
        }
        return false;
    }
}
