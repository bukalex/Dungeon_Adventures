using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public Item[] startItems;

    public int maxStackCount = 16;
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

        //Initializing slots for internal inventory
        internalInventorySlots = UIManager.Instance.inventory.GetComponentsInChildren<InventorySlot>();
        
        //Initializing slots for toolBar
        toolBar = UIManager.Instance.toolbar.GetComponentsInChildren<InventorySlot>();

        //Initializing slots for selling menu
        sellSlots = UIManager.Instance.sellSlots.GetComponentsInChildren<InventorySlot>();

        //Initializing slots for purchase menu
        storageSlots = UIManager.Instance.storage.GetComponentsInChildren<InventorySlot>(); 

        //Initializing slots for abilities
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
        for (int i = 0; i < toolBar.Length; i++)
        {
            InventorySlot slot = toolBar[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.item == item && itemInSlot.count < maxStackCount && itemInSlot.item.isStackable == true)
            {
                itemInSlot.count++;
                itemInSlot.updateCount();
                return true;
            }
            for(int j = 0; j < internalInventorySlots.Length; j++)
            {
                InventorySlot intertalSlot = internalInventorySlots[i];
                InventoryItem internalItemInSlot = intertalSlot.GetComponentInChildren<InventoryItem>();
                if(internalItemInSlot != null && internalItemInSlot.item == item && internalItemInSlot.count < maxStackCount && internalItemInSlot.item.isStackable == true)
                {
                    internalItemInSlot.count++;
                    internalItemInSlot.updateCount();
                    return true;
                }
            }
        }

        //Check if any slot has the same item with count lower than max stack
        for (int i = 0; i < toolBar.Length; i++)
        {
            InventorySlot slot = toolBar[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if(itemInSlot == null)
            {
                spawnNewItem(item, slot);
                return true;
            }

            for(int j = 0; j < internalInventorySlots.Length; j++)
            {
                InventorySlot internalSlots = internalInventorySlots[j];
                InventoryItem internalItemInSlot = internalSlots.GetComponentInChildren<InventoryItem>();
                if(internalItemInSlot != null && internalItemInSlot.count < maxStackCount && internalItemInSlot.item.isStackable == true && internalItemInSlot.item == item)
                {
                    spawnNewItem(item, slot);
                    return true;
                }
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
        InventorySlot slot = toolBar[selectedSlot];
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

