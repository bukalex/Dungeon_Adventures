using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private InventorySlot[] emp;
    public Item[] startItems;
    public Item[] allItems; 
    public Ability[] startAbilities;
    public int maxStackCount = 16;
    [Header("Slots")]
    public InventorySlot[] internalInventorySlots;
    public InventorySlot[] toolBar;
    public InventorySlot[] traderSellSlots;
    public InventorySlot[] wizardSellSlots;
    public InventorySlot[] storageSlots;
    public InventorySlot[] cheatSlots;
    public AbilitySlot[] abilityBar;
    [Header("Icon prefabs")]
    public GameObject inventoryItemPrefab, abilityItemPrefab;
    public PlayerData playerData;
    [Header("EquipmentSlots")]
    public GameObject helmetSlot;
    public GameObject glovesSlot;
    public GameObject chestplateSlot;
    public GameObject legginsSlot;
    public GameObject gemSlot;
    public GameObject swordSlot;

    public int selectedSlot = -1;
    public int[] intsd;
    public static InventoryManager Instance { get; private set; }
    public void Awake()
    {
        if (Instance == null) Instance = this;
        else DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        InitializeSlots();
        //Create all items in a cheat chest
        foreach (var item in allItems)
            AddItem(item, cheatSlots, cheatSlots);

        //Pick up items to correct inventory slot
        foreach (var item in startItems)
            AddItem(item, toolBar, internalInventorySlots);

        //Create all items in a cheat chest
        foreach (var item in allItems)
            AddItem(item, cheatSlots, cheatSlots);

        //Add bility to a abilitySlots
        foreach(var ability in startAbilities)
            AddAbility(ability);
        
        //Fill stacks of usable items in cheat chests
        foreach (var item in allItems)
            fillStacks(item, cheatSlots);
    }
    private void InitializeSlots()
    {
        //Initializing slots for internal inventory
        internalInventorySlots = UIManager.Instance.inventory.GetComponentsInChildren<InventorySlot>();
        
        //Initializing slots for toolBar
        toolBar = UIManager.Instance.toolbar.GetComponentsInChildren<InventorySlot>();

        //Initializing slots for selling menu
        traderSellSlots = UIManager.Instance.sellSlots.GetComponentsInChildren<InventorySlot>();
        wizardSellSlots = UIManager.Instance.wizardSellSlots.GetComponentsInChildren<InventorySlot>();

        //Initializing slots for purchase menu
        storageSlots = UIManager.Instance.traderStorage.GetComponentsInChildren<InventorySlot>(); 

        //Initializing slots for abilities
        abilityBar = UIManager.Instance.abilitybar.GetComponentsInChildren<AbilitySlot>();

        //Initialize slots for cheat chests
        cheatSlots = UIManager.Instance.cheatChestUIs.GetComponentsInChildren<InventorySlot>(true);

    }
    private void Update()
    {
        if (Input.anyKeyDown)
        {
            if(selectedSlot != -1) toolBar[selectedSlot].unselectSlot();

            string[] inputString = {"1", "2", "3", "4", "5", "6", "7", "8", "9"};  

            for(int i = 0; i < inputString.Length; i++)
            {
                if (Input.inputString == inputString[i])
                {
                    int.TryParse(Input.inputString, out selectedSlot);
                    selectedSlot -= 1;
                }
            }

            if (selectedSlot != -1) toolBar[selectedSlot].selectSlot();
        }
        if (Input.GetKeyUp(KeyCode.Q))
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
            if(InventoryType1 != InventoryType2)
            {
                for (int j = 0; j < InventoryType2.Length; j++)
                {
                    InventorySlot intertalSlot = InventoryType2[j];
                    InventoryItem internalItemInSlot = intertalSlot.GetComponentInChildren<InventoryItem>();
                    if (internalItemInSlot != null && internalItemInSlot.item == item && internalItemInSlot.count < maxStackCount && internalItemInSlot.item.isStackable == true)
                    {
                        internalItemInSlot.count++;
                        internalItemInSlot.updateCount();
                        return true;
                    }
                }
            }
        }
        //Check if any slot has the same item
        for (int i = 0; i < InventoryType1.Length; i++)
        {
            InventorySlot slot = InventoryType1[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if(itemInSlot == null)
            {
                spawnNewItem(item, slot);
                return true;
            }
            if(InventoryType1 != InventoryType2)
            {
                for (int j = 0; j < InventoryType2.Length; j++)
                {
                    InventorySlot internalSlots = InventoryType2[j];
                    InventoryItem internalItemInSlot = internalSlots.GetComponentInChildren<InventoryItem>();
                    if (internalItemInSlot == null)
                    {
                        spawnNewItem(item, slot);
                        return true;
                    }
                }
            }
        }
        return false;
    }
    public bool fillStacks(Item item, InventorySlot[] InventoryType)
    {
        for (int i = 0; i < InventoryType.Length; i++)
        {
            InventorySlot slot = InventoryType[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if( itemInSlot.item.isStackable == true
                && itemInSlot.count < itemInSlot.item.unitsPerStack)
            {
                itemInSlot.count = itemInSlot.item.unitsPerStack;
                itemInSlot.updateCount();
                return true;
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
                BattleManager.Instance.AssingAbility(DataManager.Instance.playerData, ability.attackParameters, abilitySlot.attackButton);
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
        if (item.GetItemType(item.itemType) == "Potion")
        {
            playerData.health += item.addHP;
            playerData.mana += item.addMP;
            playerData.speed += item.addSPD;
            playerData.stamina += item.addStamina;
        }
        else if (item.itemType == Item.ItemType.Spell)
        {
            AddAbility(item.ability);
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

