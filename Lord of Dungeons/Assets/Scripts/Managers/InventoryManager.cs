using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public Item[] startItems;

    public int maxStackCount = 16;
    public InventorySlot[] inventorySlots;
    public GameObject inventoryItemPrefab;


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
        //Change to selected slot by key code
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            changeSelectedSlot(0);
        } 
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            changeSelectedSlot(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            changeSelectedSlot(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            changeSelectedSlot(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            changeSelectedSlot(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            changeSelectedSlot(5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            changeSelectedSlot(6);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            changeSelectedSlot(7);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            changeSelectedSlot(8);
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

    

    public Item useSelectedItem(bool use)
    {
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if (itemInSlot != null) 
        {
            Item item = itemInSlot.item;
            if (use == true)
            {
                if(Input.GetKey(KeyCode.C))
                {
                    itemInSlot.count--;
                    if(itemInSlot.count <= 0)
                    {
                        Destroy(itemInSlot.gameObject);

                    }
                    else
                    {
                        itemInSlot.updateCount();
                    }
                }
            }
            return item;
        }
        return null;
    }

}

