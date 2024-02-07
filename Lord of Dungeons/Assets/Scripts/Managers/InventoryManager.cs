using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public Item[] startItems;

    public int maxStackCount = 16;
    public InventorySlot[] inventorySlots;
    public GameObject inventoryItemPrefab;
    public PlayerData playerData;


    public int selectedSlot = -1;

    private void Start()
    {
        changeSelectedSlot(0);
        foreach(var item in startItems)
        {
            AddItem(item);
        }
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            string[] inputString = { "1", "2", "3", "4", "5", "6", "7", "8", "9"};  

            for(int i = 0; i < inputString.Length; i++)
            {
                if (Input.inputString == inputString[i])
                {
                    int.TryParse(Input.inputString, out selectedSlot);
                    selectedSlot -= 1;
                }
            }
        }

        if (Input.GetKey(KeyCode.C))
        {
            useSelectedItem();
        }
    }

    void changeSelectedSlot(int newSelectedSlot)
    {
        if (selectedSlot >= 0)
        {
            selectedSlot = newSelectedSlot;
        }
    }

    public static InventoryManager instance { get; private set; }
    private void Awake()
    {
       if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            DontDestroyOnLoad (gameObject);
        }
    }
    public bool AddItem(Item item)
    {
        //check if any slot has the same item with count lower than max stack
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.item == item && itemInSlot.count < maxStackCount && itemInSlot.item.isStackable == true)
            {
                itemInSlot.count++;
                itemInSlot.updateCount();
                return true;
            }
        }

        //Check if any slot has the same item with count lower than max stack
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if(itemInSlot == null)
            {
                spawnNewItem(item, slot);
                return true;
            }
        }

        return false;
    }
    
    public void spawnNewItem(Item item, InventorySlot slot)
    {
        GameObject newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItemGo.GetComponent<InventoryItem>();
        inventoryItem.InitializeItem(item);
    }

    public void itemIsUsed(Item item)
    {
        if (item.isUsable == true)
        {
            playerData.health += item.addHP;
            playerData.speed += item.addSPD;
            if (playerData.speed ==playerData.speed + item.addSPD)
            {
                new WaitForSeconds(item.cooldown);
                playerData.speed -= item.addSPD;
            }
            playerData.health -= item.addStamina;
        }
    }

    public Item useSelectedItem()
    {
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot != null) 
        {
            Item item = itemInSlot.item;
            if (item.isUsable == true)
            {
                    itemInSlot.count--;
                    if(itemInSlot.count <= 0)
                    {
                        Destroy(itemInSlot.gameObject);
                        itemIsUsed(item);
                    }
                    else
                    {
                        itemInSlot.updateCount();
                    }
            }
            return item;
        }
        return null;
    }

}

