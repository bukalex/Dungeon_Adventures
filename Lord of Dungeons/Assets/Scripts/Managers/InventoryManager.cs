using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject ItemDescription;
    private InventorySlot[] emp;
    [Header("Loot Tables")]  
    public Item[] startItems;
    public Item[] allItems;
    public Item[] materialLootTable;
    public Item[] weaponLootTable;
    public Item[] consumableLootTable;
    public Item[] uniqueLootTable;
    public List<Item> randomLootTable = new List<Item>();
    public List<Item> randomMaterialLootTable = new List<Item>();
    public List<Item> randomWeaponLootTable = new List<Item>();
    public List<Item> randomConsumableLootTable = new List<Item>();
    public List<Item> randomUniqueLootTable = new List<Item>();
    public Ability[] startAbilities;
    public Ability[] allAbilities;
    [Header("Slots")]
    public InventorySlot[] internalInventorySlots;
    public InventorySlot[] toolBar;
    public InventorySlot[] equipmentSlots;
    public InventorySlot[] traderSellSlots;
    public InventorySlot[] wizardSellSlots;
    public InventorySlot[] storageSlots;
    public InventorySlot[] cheatSlots;
    public AbilitySlot[] abilityBar;
    public AbilitySlot[] abilityInventory;
    [Header("Icon prefabs")]
    public GameObject inventoryItemPrefab, abilityItemPrefab, abilitySlotPrefab;
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
    public int activeAbilities = 0;

    public static InventoryManager Instance { get; private set; }
    public void Awake()
    {
        if (Instance == null) Instance = this;
    }
    private void Start()
    {
        InitializeSlots();
        //Create all items in a cheat chest
        foreach (var item in allItems)
            AddItem(item, cheatSlots, cheatSlots);

        //Create Random Loot Table from all items in the game
        //foreach (var item in allItems)
        //    FillRandomLootTable(item);

        //Pick up items to correct inventory slot
        foreach (var item in startItems)
            AddItem(item, toolBar, internalInventorySlots);

        //adds random items from all items in the game to the inventory of the player
        //foreach (var item in randomLootTable)
        //   AddItem(item, cheatSlots, cheatSlots);

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
    public void InitializeSlots()
    {
        //Initializing slots for internal inventory
        internalInventorySlots = UIManager.Instance.inventory.transform.GetChild(3).GetComponentsInChildren<InventorySlot>();
        equipmentSlots = UIManager.Instance.equipment.GetComponentsInChildren<InventorySlot>();

        //Initializing slots for toolBar
        toolBar = UIManager.Instance.toolbar.GetComponentsInChildren<InventorySlot>();

        //Initializing slots for selling menu
        traderSellSlots = UIManager.Instance.sellSlots.GetComponentsInChildren<InventorySlot>();
        wizardSellSlots = UIManager.Instance.wizardSellSlots.GetComponentsInChildren<InventorySlot>();

        //Initializing slots for purchase menu
        storageSlots = UIManager.Instance.traderStorage.GetComponentsInChildren<InventorySlot>(); 

        //Initializing slots for abilities
        abilityBar = UIManager.Instance.abilitybar.GetComponentsInChildren<AbilitySlot>();
        //abilityInventory = UIManager.Instance.abilityInventory.GetComponentsInChildren<AbilitySlot>();

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
        CheckEquipmentSlots();
    }
    private void CheckEquipmentSlots()
    {
        PlayerData playerData = DataManager.Instance.playerData;
        List<InventoryItem> equipmentItems = new List<InventoryItem>();
        if (helmetSlot.GetComponentInChildren<InventoryItem>() != null) equipmentItems.Add(helmetSlot.GetComponentInChildren<InventoryItem>());
        if (glovesSlot.GetComponentInChildren<InventoryItem>() != null) equipmentItems.Add(glovesSlot.GetComponentInChildren<InventoryItem>());
        if (chestplateSlot.GetComponentInChildren<InventoryItem>() != null) equipmentItems.Add(chestplateSlot.GetComponentInChildren<InventoryItem>());
        if (legginsSlot.GetComponentInChildren<InventoryItem>() != null) equipmentItems.Add(legginsSlot.GetComponentInChildren<InventoryItem>());
        if (gemSlot.GetComponentInChildren<InventoryItem>() != null) equipmentItems.Add(gemSlot.GetComponentInChildren<InventoryItem>());
        if (swordSlot.GetComponentInChildren<InventoryItem>() != null) equipmentItems.Add(swordSlot.GetComponentInChildren<InventoryItem>());

        playerData.attack = playerData.initialAttack;
        playerData.defense = playerData.initialDefense;
        playerData.specialAttack = playerData.initialSpecialAttack;
        playerData.specialDefense = playerData.initialSpecialDefense;
        foreach (InventoryItem item in equipmentItems)
        {
            playerData.attack += item.item.addAttack;
            playerData.defense += item.item.addDefense;
            playerData.specialAttack += item.item.addSpecialAttack;
            playerData.specialDefense += item.item.addSpecialDefense;
        }
        foreach (InventoryItem item in equipmentItems)
        {
            playerData.attack *= item.item.increseAttack;
            playerData.defense *= item.item.increaseDefense;
            playerData.specialAttack *= item.item.increseSpecialAttack;
            playerData.specialDefense *= item.item.increaseSpecialDefense;
        }
    }
    public bool AddItem(Item item, InventorySlot[] InventoryType1, InventorySlot[] InventoryType2)
    {
        //check if any slot has the same item with count lower than max stack
        for (int i = 0; i < InventoryType1.Length; i++)
        {
            InventorySlot slot = InventoryType1[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.item == item && itemInSlot.count < item.unitsPerStack && itemInSlot.item.isStackable == true)
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
                    if (internalItemInSlot != null && internalItemInSlot.item == item && internalItemInSlot.count < item.unitsPerStack && internalItemInSlot.item.isStackable == true)
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
        }
        if (InventoryType1 != InventoryType2)
        {
            for (int j = 0; j < InventoryType2.Length; j++)
            {
                InventorySlot internalSlots = InventoryType2[j];
                InventoryItem internalItemInSlot = internalSlots.GetComponentInChildren<InventoryItem>();
                if (internalItemInSlot == null)
                {
                    spawnNewItem(item, internalSlots);
                    return true;
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
            if(itemInSlot != null &&
                itemInSlot.item.isStackable == true
                && itemInSlot.count < itemInSlot.item.unitsPerStack)
            {
                itemInSlot.count = itemInSlot.item.unitsPerStack;
                itemInSlot.updateCount();
                return true;
            }
        }
        return false;
    }
    public void InitializeItemDescription(Item item)
    {

    }

    public bool HasCoins()
    {
        foreach (InventorySlot slot in toolBar)
        {
            InventoryItem inventoryItem = slot.GetComponentInChildren<InventoryItem>();
            if (inventoryItem != null && inventoryItem.item.itemType == Item.ItemType.Coin)
            {
                return true;
            }
        }

        foreach (InventorySlot slot in internalInventorySlots)
        {
            InventoryItem inventoryItem = slot.GetComponentInChildren<InventoryItem>();
            if (inventoryItem != null && inventoryItem.item.itemType == Item.ItemType.Coin)
            {
                return true;
            }
        }

        return false;
    }

    public bool HasSpace(Item item)
    {
        for (int i = 0; i < toolBar.Length; i++)
        {
            InventorySlot slot = toolBar[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null && itemInSlot.item == item && itemInSlot.count < item.unitsPerStack && itemInSlot.item.isStackable == true)
            {
                return true;
            }
            for (int j = 0; j < internalInventorySlots.Length; j++)
            {
                InventorySlot intertalSlot = internalInventorySlots[j];
                InventoryItem internalItemInSlot = intertalSlot.GetComponentInChildren<InventoryItem>();
                if (internalItemInSlot != null && internalItemInSlot.item == item && internalItemInSlot.count < item.unitsPerStack && internalItemInSlot.item.isStackable == true)
                {
                    return true;
                }
            }
        }
        //Check if any slot has the same item
        for (int i = 0; i < toolBar.Length; i++)
        {
            InventorySlot slot = toolBar[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {
                return true;
            }
        }
        for (int j = 0; j < internalInventorySlots.Length; j++)
        {
            InventorySlot internalSlots = internalInventorySlots[j];
            InventoryItem internalItemInSlot = internalSlots.GetComponentInChildren<InventoryItem>();
            if (internalItemInSlot == null)
            {
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

        foreach (AbilitySlot abilitySlot in abilityInventory)
        {
            AbilityItem abilityInSlot = abilitySlot.GetComponentInChildren<AbilityItem>();
            if (abilityInSlot == null)
            {
                spawnNewAbility(ability, abilitySlot);
                return true;
            }
        }
        AbilitySlot slot = Instantiate(abilitySlotPrefab, UIManager.Instance.abilityInventory.transform).GetComponent<AbilitySlot>();
        abilityInventory = UIManager.Instance.abilityInventory.GetComponentsInChildren<AbilitySlot>(true);
        spawnNewAbility(ability, slot);
        return false;
    }
    private bool HasAbility(Ability ability)
    {
        foreach (AbilitySlot abilitySlot in abilityBar)
        {
            AbilityItem abilityInSlot = abilitySlot.GetComponentInChildren<AbilityItem>();
            if (abilityInSlot != null && abilityInSlot.ability.abilityName == ability.abilityName)
            {
                abilityInSlot.ability.attackParameters.SetRank(abilityInSlot.ability.attackParameters.rank + 1);
                DataManager.Instance.playerData.attacks[DataManager.Instance.playerData.type][abilitySlot.attackButton].SetRank(abilityInSlot.ability.attackParameters.rank);
                return true;
            }
        }

        foreach (AbilitySlot abilitySlot in abilityInventory)
        {
            AbilityItem abilityInSlot = abilitySlot.GetComponentInChildren<AbilityItem>();
            if (abilityInSlot != null && abilityInSlot.ability.abilityName == ability.abilityName)
            {
                abilityInSlot.ability.attackParameters.SetRank(abilityInSlot.ability.attackParameters.rank + 1);
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
    public void spawnNewAbility(Ability ability, AbilitySlot slot, int rank = 1)
    {
        GameObject newAbilityGo = Instantiate(abilityItemPrefab, slot.transform);
        AbilityItem abilityItem = newAbilityGo.GetComponent<AbilityItem>();
        abilityItem.InitializeAbility(ability, rank); 
    }
    public void itemIsUsed(Item item)
    {
        if (item.itemType == Item.ItemType.Potion)
        {
            playerData.health += item.addHP;
            playerData.mana += item.addMP;
            playerData.speed += item.addSPD;
            playerData.stamina += item.addStamina;
        }
        else if (item.itemType == Item.ItemType.Spell)
        {
            if (!HasAbility(item.ability)) AddAbility(item.ability);
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
                itemIsUsed(item);
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
            return item;
        }
        return null;
    }
    public Ability useSelectedAbility()
    {
        return null;
    }
    public bool FillRandomLootTable(Item item)
    {
        List<Item> exampleLootPool = new List<Item>();
        int randomNum = Random.Range(0, 101);


        if (randomNum <= item.rarity)
        {
            randomLootTable.Add(item);

            Debug.Log("Rolled: " + randomNum + ". And added: " + item + ". which has a rarity of: " + item.rarity);
        }
        else
        {
            Debug.Log("Rolled: " + randomNum + ". And thus could not add:" + item + ". which has a rarity of: " + item.rarity);
        }
        return false;
    }

    public void LoseInventory()
    {
        foreach (InventorySlot slot in toolBar)
        {
            InventoryItem inventoryItem = slot.GetComponentInChildren<InventoryItem>();
            if (inventoryItem != null)
            {
                Destroy(inventoryItem.gameObject);
            }
        }

        foreach (InventorySlot slot in internalInventorySlots)
        {
            InventoryItem inventoryItem = slot.GetComponentInChildren<InventoryItem>();
            if (inventoryItem != null)
            {
                Destroy(inventoryItem.gameObject);
            }
        }
    }

    public int CountAbilities()
    {
        int count = 0;
        foreach (AbilitySlot slot in abilityBar)
        {
            if (slot.GetComponentInChildren<AbilityItem>() != null) count++;
        }
        foreach (AbilitySlot slot in abilityInventory)
        {
            if (slot.GetComponentInChildren<AbilityItem>() != null) count++;
        }
        return count;
    }

    public void LoadItem(InventorySlot[] slots, int slotIndex, int itemIndex, int itemCount)
    {
        if (itemIndex != -1)
        {
            InventoryItem item = Instantiate(inventoryItemPrefab, slots[slotIndex].transform).GetComponent<InventoryItem>();
            item.count = itemCount;
            item.InitializeItem(allItems[itemIndex]);
        }
    }

    public void LoadAbilityInventory(int slotCount)
    {
        if (slotCount == 0) slotCount = 8;
        for (int i = 0; i < slotCount; i++)
        {
            Instantiate(abilitySlotPrefab, UIManager.Instance.abilityInventory.transform);
        }
        abilityInventory = UIManager.Instance.abilityInventory.GetComponentsInChildren<AbilitySlot>(true);
    }

    public void LoadAbility(AbilitySlot[] slots, int slotIndex, int abilityIndex, int rank)
    {
        if (abilityIndex != -1)
        {
            spawnNewAbility(allAbilities[abilityIndex], slots[slotIndex], rank);
            if (slots[slotIndex].attackButton != BattleManager.AttackButton.NONE)
            {
                BattleManager.Instance.AssingAbility(DataManager.Instance.playerData, allAbilities[abilityIndex].attackParameters, slots[slotIndex].attackButton);
                DataManager.Instance.playerData.attacks[DataManager.Instance.playerData.type][slots[slotIndex].attackButton].SetRank(rank);
            }
        }
    }

    public void StartCooldown(BattleManager.AttackButton attackButton)
    {
        if (attackButton != BattleManager.AttackButton.NONE &&
                attackButton != BattleManager.AttackButton.LMB &&
                attackButton != BattleManager.AttackButton.RMB)
        {
            foreach (AbilitySlot slot in abilityBar)
            {
                if (slot.attackButton == attackButton)
                {
                    slot.GetComponentInChildren<AbilityItem>().timer = 0;
                    break;
                }
            }
        }
    }
}

