using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public Item[] startItems;

    public int maxStackCount = 16;
    public InventorySlot[] inventorySlots;
    public InventorySlot[] internalInventorySlots;
    public InventorySlot[] toolBar;
    public InventorySlot[] Ability;
    public InventorySlot[] sellSlots;
    public InventorySlot[] storageSlots;
    public GameObject inventoryItemPrefab;
    public PlayerData playerData;


    public int selectedSlot = -1;

    public static InventoryManager Instance { get; private set; }

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }


    }
    private void Start()
    {
        InitializeSlots();
        foreach(var item in startItems)
        {
            AddItem(item);
        }
    }

    private void InitializeSlots()
    {
        //Initializing slots for the whole inventory
        
        //foreach (InventorySlot slot in inventorySlots)
        //{
        //    for (int i = 0; i < inventorySlots.Length; i++)
        //    {
        //        
        //        inventorySlots[i] = UIManager.Instance.inventory.GetComponentInChildren<InventorySlot>();
        //        
        //    }
        //}
        inventorySlots = UIManager.Instance.inventory.GetComponentsInChildren<InventorySlot>();

        //Initializing slots for internal inventory
        //foreach (InventorySlot slot in internalInventorySlots)
        //{
        //    for (int i = 0; i < internalInventorySlots.Length; i++)
        //    {
        //       
        //        internalInventorySlots[i] = UIManager.Instance.internalInventory.GetComponentInChildren<InventorySlot>();
        //       
        //    }
        //}
        internalInventorySlots = UIManager.Instance.inventory.GetComponentsInChildren<InventorySlot>();
        
        //Initializing slots for toolBar
        //foreach (InventorySlot slot in toolBar)
        //{
        //    for (int i = 0; i < toolBar.Length; i++)
        //    {
        //        
        //        toolBar[i] = UIManager.Instance.toolbar.GetComponentInChildren<InventorySlot>();
        //        
        //    }
        //}
        toolBar = UIManager.Instance.toolbar.GetComponentsInChildren<InventorySlot>();

        //Initializing slots for selling menu
        foreach (InventorySlot slot in sellSlots)
        {
            for (int i = 0; i < sellSlots.Length; i++)
            {
                
                sellSlots[i] = UIManager.Instance.sellSlots.GetComponentInChildren<InventorySlot>();
                
            }
        }
        sellSlots = UIManager.Instance.sellSlots.GetComponentsInChildren<InventorySlot>();

        //Initializing slots for purchase menu
        foreach (InventorySlot slot in storageSlots)
        {
            for (int i = 0; i < storageSlots.Length; i++)
            {
               
                storageSlots[i] = UIManager.Instance.storage.GetComponentInChildren<InventorySlot>();
               
            }
        }
        storageSlots = UIManager.Instance.storage.GetComponentsInChildren<InventorySlot>(); 

        ////Initializing slots for abilities
        //foreach (InventorySlot slot in Ability)
        //{
        //    for (int i = 0; i < Ability.Length; i++)
        //    {
        //        {
        //            Ability[i] = UIManager.Instance.storage.GetComponentInChildren<InventorySlot>();
        //        }
        //    }
        //}
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

