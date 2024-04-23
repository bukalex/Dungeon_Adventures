using System.Collections;
using Assets.Enums.ItemEnums;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using System;
using Random = UnityEngine.Random;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    public WeaponType currentWeaponType;

    public GameObject ItemDescription;
    public InventorySlot[] emp;
    public SpriteCollection spriteCollection;
    public SpriteAtlas hairAtlas;
    public List<PlayerDirection> directions = new List<PlayerDirection>();
    public List<GameObject> inventorySlots = new List<GameObject>();
   
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
    public InventorySlot inventorySlotPrefab;
    public InventorySlot deleteSlot;
    public InventorySlot[] internalInventorySlots;
    public InventorySlot[] toolBar;
    public InventorySlot[] equipmentSlots;
    public InventorySlot[] traderSellSlots;
    public InventorySlot[] wizardSellSlots;
    public InventorySlot[] storageSlots;
    public InventorySlot[] cheatSlots;
    public InventorySlot[] allActiveSlots;
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

    public InventorySlot helmet;

    public int selectedSlot = 0;
    public int activeAbilities = 0;

    private List<Item> items = new List<Item>();
    public Item itemToChange;
    public InventoryItem currentInventoryItem;
    private bool isEmpty = true;

    public static InventoryManager Instance { get; private set; }
    public void Awake()
    {
        if (Instance == null) Instance = this;
    }
    private void Start()
    {
        InitializeSlots();

        foreach(InventorySlot slot in equipmentSlots)
        {
            slot.slotTag = slot.tag;
        }

        //Create all items in a cheat chest
        foreach (var item in allItems)
            AddItem(item, cheatSlots.ToList(), cheatSlots.ToList());

        //Pick up items to correct inventory slot
        foreach (var item in startItems)
            AddItem(item, toolBar.ToList(), internalInventorySlots.ToList());

        //Create all items in a cheat chest
        foreach (var item in allItems)
            AddItem(item, cheatSlots.ToList(), cheatSlots.ToList());

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
        internalInventorySlots = UIManager.Instance.inventory.transform.GetChild(2).GetComponentsInChildren<InventorySlot>();
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

        EquipEvent();

        foreach (InventorySlot slot in equipmentSlots)
        {
           if(slot.transform.childCount == 0)
            {
                switch (slot.slotTag)
                {
                    case "Sword":
                        directions.ForEach(i => i.MainWeapon.sprite = null);
                        directions.ForEach(i => i.Handle.sprite = null);
                        directions.ForEach(i => i.LimbL.sprite = null);
                        directions.ForEach(i => i.LimbU.sprite = null);
                        break;
                    case "Boots":
                        directions.ForEach(i => i.LeftLeg.sprite = null);
                        directions.ForEach(i => i.RightLeg.sprite = null);
                        break;
                    case "Chestplate":
                        directions.ForEach(i => i.Armor.sprite = null);
                        break;
                    case "Helmet":
                        directions.ForEach(i => i.Hair.color += new Color(0, 0, 0, 1));
                        directions.ForEach(i => i.Helmet.sprite = null);
                        break;
                    case "Gloves":
                        directions.ForEach(i => i.LeftArm.sprite = null);
                        directions.ForEach(i => i.RightArm.sprite = null);
                        break;
                }
            }
        }

        InventorySlot helmSlot = helmetSlot.GetComponent<InventorySlot>();
        if (helmetSlot.transform.childCount == 1)
            directions.ForEach(i => i.Hair.sprite = null);
        else
            directions.ForEach(i => i.Hair.sprite = i.Hair.GetComponent<SpriteMapping>().FindSprite(hairAtlas));

        if (Input.GetMouseButtonDown(1))
        {
            MoveItemSilently(itemToChange);
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (currentInventoryItem != null && Input.GetMouseButtonDown(0) && itemToChange == currentInventoryItem.GetComponent<InventoryItem>().item)
            {
                if (TrainingManager.Instance != null) TrainingManager.Instance.itemWasClickedAndMoved = true;
                InstantlyMoveItem(itemToChange, toolBar, internalInventorySlots);
            }
        }
        if (ItemDescription.activeSelf)
        {
            ItemDescription.transform.position = Input.mousePosition + new Vector3(250f, 200f);
            if (Input.mousePosition.x + ItemDescription.GetComponent<RectTransform>().sizeDelta.x > Screen.width)
            {
                ItemDescription.transform.position += Vector3.left * ItemDescription.GetComponent<RectTransform>().sizeDelta.x + new Vector3(150f, 0);
            }
            if (Input.mousePosition.y + ItemDescription.GetComponent<RectTransform>().sizeDelta.y > Screen.height)
            {
                ItemDescription.transform.position += Vector3.down * ItemDescription.GetComponent<RectTransform>().sizeDelta.y;
            }
        }

        toolBar[selectedSlot].unselectSlot();
        if (Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) selectedSlot = 0;
            if (Input.GetKeyDown(KeyCode.Alpha2)) selectedSlot = 1;
            if (Input.GetKeyDown(KeyCode.Alpha3)) selectedSlot = 2;
            if (Input.GetKeyDown(KeyCode.Alpha4)) selectedSlot = 3;
            if (Input.GetKeyDown(KeyCode.Alpha5)) selectedSlot = 4;
            if (Input.GetKeyDown(KeyCode.Alpha6)) selectedSlot = 5;
            if (Input.GetKeyDown(KeyCode.Alpha7)) selectedSlot = 6;
            if (Input.GetKeyDown(KeyCode.Alpha8)) selectedSlot = 7;
            if (Input.GetKeyDown(KeyCode.Alpha9)) selectedSlot = 8;
        }

        if (!UIManager.Instance.npcWindowActive) selectedSlot = (selectedSlot + (int)Input.mouseScrollDelta.y + 9) % 9;
        toolBar[selectedSlot].selectSlot();

        if (Input.GetKeyUp(UIManager.Instance.keyCodes[14]) && (TrainingManager.Instance == null || TrainingManager.Instance != null && !TrainingManager.Instance.itemUsageBlocked))
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
    public bool AddItem(Item item, List<InventorySlot> InventoryType1, List<InventorySlot> InventoryType2)
    {
        InventorySlot slotInToolbar = InventoryType1.Find(slot => slot.transform.childCount == 0);
        InventorySlot slotInventory2 = InventoryType2.Find(slot => slot.transform.childCount == 0);

        InventoryItem itemInToolbar = InventoryType1.Find(slot => slot.GetComponentInChildren<InventoryItem>()?.item == item)?.GetComponentInChildren<InventoryItem>();
        InventoryItem itemInventory2 = InventoryType2.Find(slot => slot.GetComponentInChildren<InventoryItem>()?.item == item)?.GetComponentInChildren<InventoryItem>();

        Item toolbarItem = itemInToolbar?.item ?? item;
        Item internalItem = itemInventory2?.item ?? item;

        if (item.isStackable
            && itemInToolbar != null && itemInToolbar.item == item
            && itemInToolbar.count < item.unitsPerStack && itemInToolbar.item.isStackable)
        {
            itemInToolbar.count++;
            itemInToolbar.updateCount();

            return true;
        }
        else if (item.isStackable 
            && itemInventory2 != null && itemInventory2.item == item
            && itemInventory2.count < item.unitsPerStack && itemInventory2.item.isStackable)
        {
            itemInventory2.count++;
            itemInventory2.updateCount();

            return true;
        }

        if (slotInToolbar != null || slotInventory2 != null)
        {
            spawnNewItem(item, slotInToolbar ?? slotInventory2);
            items.Add(item);

            return true;
        }

        return false;
    }

    public void EquipEvent()
    {
        foreach (InventorySlot slot in equipmentSlots)
        {
            StartCoroutine(onSlotEquiped(slot, () =>
            {
                if (slot.transform.childCount > 0)
                {
                    Item item = slot.GetComponentInChildren<InventoryItem>().item;
                    if (item.itemType == Item.ItemType.Weapon)
                        EquipeWeapon(item);
                    else
                        EquipeArmor(item);
                }
            }));
        }
    }

    public void EquipeArmor(Item item)
    {
        SpriteAtlas armorSprite = spriteCollection.Armors[item.atlasID].Sprites;

        switch (item.bodyPart)
        {
            case BodyPartName.Body:
                directions.ForEach(i => i.Armor.sprite = i.Armor.GetComponent<SpriteMapping>().FindSprite(armorSprite));
                break;
            case BodyPartName.Legs:
                directions.ForEach(i => i.LeftLeg.sprite = i.LeftLeg.GetComponent<SpriteMapping>().FindSprite(armorSprite));
                directions.ForEach(i => i.RightLeg.sprite = i.RightLeg.GetComponent<SpriteMapping>().FindSprite(armorSprite));
                break;
            case BodyPartName.Arms:
                directions.ForEach(i => i.LeftArm.sprite = i.LeftArm.GetComponent<SpriteMapping>().FindSprite(armorSprite));
                directions.ForEach(i => i.RightArm.sprite = i.RightArm.GetComponent<SpriteMapping>().FindSprite(armorSprite));
                break;
            case BodyPartName.Head:
                directions.ForEach(i => i.Helmet.sprite = i.Helmet.GetComponent<SpriteMapping>().FindSprite(armorSprite));
                directions.ForEach(i => i.Helmet.sprite = i.Helmet.GetComponent<SpriteMapping>().FindSprite(armorSprite));
                break;
        }
    }

    public void EquipeWeapon(Item item)
    {
        switch (item.weaponType)
        {
            case WeaponType.Bow:
                directions.ForEach(i => i.Handle.sprite = i.Handle.GetComponent<SpriteMapping>().FindSprite(spriteCollection.Bows[item.atlasID].Sprites));
                directions.ForEach(i => i.LimbU.sprite = i.LimbU.GetComponent<SpriteMapping>().FindSprite(spriteCollection.Bows[item.atlasID].Sprites));
                directions.ForEach(i => i.LimbL.sprite = i.LimbL.GetComponent<SpriteMapping>().FindSprite(spriteCollection.Bows[item.atlasID].Sprites));
                currentWeaponType = WeaponType.Bow;
                break;
            case WeaponType.Sword:
                directions.ForEach(i => i.MainWeapon.sprite = spriteCollection.Swords[item.atlasID].Sprite);
                currentWeaponType = WeaponType.Sword;
                break;
        }
    }
    public void DeleteItem()
    {
        InventoryItem itemInTrashcan = deleteSlot.GetComponentInChildren<InventoryItem>();
        Destroy(itemInTrashcan.transform.gameObject);
    }
    public void MoveItemSilently(Item item)
    {
        //spawnNewItem(item)
    }
    public void InstantlyMoveItem(Item item, InventorySlot[] toolbar, InventorySlot[] inventoryType2)
    {
        int itemPosition = -1;
        for(int i = 0; i < toolbar.Length; i++)
        {
            InventorySlot slot = toolbar[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
                if (itemInSlot != null && itemInSlot == currentInventoryItem)
                {
                    itemPosition = 0;
                }
        }
        if (itemPosition == -1)
        {
            for (int i = 0; i < inventoryType2.Length; i++)
            {
                InventorySlot slot = inventoryType2[i];
                InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
                if (itemInSlot != null)
                {
                    itemPosition = 1;
                    break;
                }
            }
        }
        if(itemPosition == 0)
        {
            for (int i = 0; i < inventoryType2.Length; i++)
            {
                InventorySlot slot = inventoryType2[i];
                InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
                if (itemInSlot == null)
                {
                    currentInventoryItem.transform.parent = slot.transform;
                    break;
                }
            }
        }

        if (itemPosition == 1)
        {
            for (int i = 0; i < toolbar.Length; i++)
            {
                InventorySlot slot = toolbar[i];
                InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
                if (itemInSlot == null)
                {
                    currentInventoryItem.transform.parent = slot.transform;
                    break;
                }
            }
        }
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
        ItemDescription.transform.GetChild(0).GetComponent<TMP_Text>().text = item.name;
        ItemDescription.transform.GetChild(1).GetComponent<TMP_Text>().text = item.description.Replace("\\n", "\n");
        ItemDescription.transform.GetChild(2).GetComponent<TMP_Text>().text = item.itemType.ToString();
    }
    public void InitializeAbilityDescription(Ability ability)
    {
        ItemDescription.transform.GetChild(0).GetComponent<TMP_Text>().text = ability.abilityName;
        ItemDescription.transform.GetChild(1).GetComponent<TMP_Text>().text = ability.description.Replace("\\n", "\n");
        ItemDescription.transform.GetChild(2).GetComponent<TMP_Text>().text = "Level " + ability.attackParameters.rank;
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
    public void spawnNewItem(Item item)
    {
        GameObject newItemGo = Instantiate(inventoryItemPrefab);
        InventoryItem inventoryItem = newItemGo.GetComponent<InventoryItem>();
        inventoryItem.InitializeItem(item);
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
            if (TrainingManager.Instance != null) TrainingManager.Instance.abilityLearned = true;
        }
        else if (item.itemType == Item.ItemType.Food)
        {
            Camera.main.GetComponent<Animator>().SetTrigger("rotate");
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
    private IEnumerator onSlotEquiped(InventorySlot equipmentSlot, Action callback)
    {
        yield return new WaitUntil(() => equipmentSlot != null);

        callback();
    }
}