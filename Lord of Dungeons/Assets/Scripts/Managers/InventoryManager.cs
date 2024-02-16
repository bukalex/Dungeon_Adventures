using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public Item[] startItems;
    public Ability[] startAbilities;

    public int maxStackCount = 16;
    public InventorySlot[] internalInventorySlots;
    public InventorySlot[] toolBar;
    public InventorySlot[] sellSlots;
    public InventorySlot[] storageSlots;
    public AbilitySlot[] abilityBar;
    public GameObject inventoryItemPrefab, abilityItemPrefab;
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
            //AddItem(item);

        foreach(var ability in startAbilities)
            AddAbility(ability);
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
        abilityBar = UIManager.Instance.abilitybar.GetComponentsInChildren<AbilitySlot>();
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
    public bool AddItem(Item item, InventorySlot[] InventoryType1, InventorySlot[] InventoryType2)
    {
        //check if any slot has the same item with count lower than max stack
        for (int i = 0; i < InventoryType1.Length; i++)
        {
            InventorySlot slot = InventoryType1[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.item == item && itemInSlot.count < maxStackCount && itemInSlot.item.isStackable == true)
            {
                itemInSlot.count++;
                itemInSlot.updateCount();
                return true;
            }
            for(int j = 0; j < InventoryType2.Length; j++)
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
        //Check if any slot has the same item
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
                if(internalItemInSlot == null)
                {
                    spawnNewItem(item, slot);
                    return true;
                }
            }
        }

        return false;
    }

    public bool AddAbility(Ability ability)
    {
        for(int i = 0; i  < abilityBar.Length; i++)
        {
            AbilitySlot abilitySlot = abilityBar[i];
            AbilityItem abilityInSlot = abilitySlot.GetComponentInChildren<AbilityItem>();
            if(abilityInSlot == null)
            {
                spawnNewAbility(ability, abilitySlot);
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
    public void spawnNewAbility(Ability ability, AbilitySlot slot)
    {
        GameObject newAbilityGo = Instantiate(abilityItemPrefab, slot.transform);
        AbilityItem abilityItem = newAbilityGo.GetComponent<AbilityItem>();
        abilityItem.InitializeAbility(ability); 
    }
    public void itemIsUsed(Item item)
    {
        if (item.isUsable == true)
        {
            playerData.health += item.addHP;
            playerData.speed += item.addSPD;    
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

    public Ability useSelectedAbility()
    {
        return null;
    }

}

