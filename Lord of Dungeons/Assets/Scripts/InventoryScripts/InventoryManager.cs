using Assets.Scripts.Data;
using Assets.Scripts.Enums;
using Assets.Scripts.InventoryElements;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventorySlotPrefab;
    public GameObject inventoryItemPrefab;
    public List<Item> items = new List<Item>();
    public ItemCollection itemCollection;

    public List<InventorySlot> inventorySlots;
    public List<InventorySlot[]> slotHolders = new List<InventorySlot[]>();

    public List<InventorySlot> ToolBar = new List<InventorySlot>();

    public int selectedSlot = -1;
    private const int ITEMAREA = 10000;
    public static InventoryManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        for(int i = 0; i < 2; i++)
        {
            foreach (Item item in itemCollection.Items)
                AddItem(item);
        }
        foreach (InventorySlot[] slotHolder in slotHolders)
        {
            InventorySlot slot = slotHolder.ToList().Find(slot => slot.transform.parent.parent.name == "ToolBar");
            if(slot != null)
            {
                ToolBar = slotHolder.ToList();
                break;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UseSelectedItem();
        }

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
    }

    public void Initialize()
    {
        foreach (GameObject holder in GameObject.FindGameObjectsWithTag("SlotHolder"))
        {
            RectTransform rt = holder.GetComponent<RectTransform>();
            int holderArea = (int)((rt.sizeDelta.x * rt.localScale.x) * (rt.sizeDelta.y * rt.localScale.y));

            for (int i = 0; i < (holderArea / ITEMAREA); i++)
            {
                GameObject go = Instantiate(inventorySlotPrefab, holder.transform);
                InventorySlot slot = go.GetComponent<InventorySlot>();
                inventorySlots.Add(slot);
            }

            slotHolders.Add(inventorySlots.ToArray());
            inventorySlots.Clear();
        }
    }

    public void AddItem(Item item)
    {
        Item existingItem = items.Find(Item => item.name == Item.name);

        if (existingItem == null)
        {
            items.Add(item);
            foreach (InventorySlot[] slotHolder in slotHolders)
            {
                InventorySlot Slot = slotHolder.ToList().Find(slot => slot.transform.childCount == 0);
                SpawnNewItem(item, Slot);
                break;
            }
        }
        else
        {
            existingItem.inventoryItemPref.GetComponent<InventoryItem>().updateCount(item, existingItem);
        }
    }

    public void UseSelectedItem() 
    {
        InventorySlot slotToUse = ToolBar[selectedSlot];
        InventoryItem itemInSlot = slotToUse.GetComponentInChildren<InventoryItem>();

        if (itemInSlot.isUsable)
        {
            List<ModifierID> modifiers = itemInSlot.GetItemModifiers(itemInSlot.item);
            ModifierValues modifierValues = new ModifierValues();

            foreach (ModifierID modifier in modifiers)
            {
                itemCollection.playerData = (PlayerData)GetStatsChanges(modifier);
            }


            if ((int.Parse(itemInSlot.countText.text) > 0))
                itemInSlot.countText.text = (int.Parse(itemInSlot.countText.text) - 1).ToString();

            if ((int.Parse(itemInSlot.countText.text) == 0))
                Destroy(gameObject);
        }
    }

    public InventorySlot[] GetHolder(string holderName)
    {
        foreach (InventorySlot[] slotHolder in slotHolders)
        {
            InventorySlot slot = slotHolder.ToList().Find(slot => slot.transform.parent.parent.name == holderName);
            if (slot != null)
                return slotHolder;
        }
        
        return null;
    }
    public object GetStatsChanges(ModifierID modID)
    {
        var Value = new Hashtable
            {
                {ModifierID.HealthRestore, itemCollection.playerData.health += (float)ModifierID.HealthRestore},
                {ModifierID.HealthRecovery, itemCollection.playerData.healthRestoreRate += (float)ModifierID.HealthRecovery},
                {ModifierID.HealthMax, itemCollection.playerData.maxHealth += (float)ModifierID.HealthMax},
                {ModifierID.ManaRestore, itemCollection.playerData.mana += (float)ModifierID.ManaRestore},
                {ModifierID.ManaRecovery, itemCollection.playerData.manaRestoreRate += (float)ModifierID.ManaRecovery},
                {ModifierID.ManaMax, itemCollection.playerData.maxMana += (float)ModifierID.ManaMax},
                {ModifierID.StaminaRestore, itemCollection.playerData.stamina += (float)ModifierID.StaminaRestore},
                {ModifierID.StaminaRecovery, itemCollection.playerData.staminaRestoreRate += (float)ModifierID.StaminaRecovery},
                {ModifierID.StaminaMax, itemCollection.playerData.maxStamina += (float)ModifierID.StaminaMax},
                {ModifierID.AttackIncrease, itemCollection.playerData.attack += (float)ModifierID.AttackIncrease},
                {ModifierID.AttackDecrease, itemCollection.playerData.attack -= (float)ModifierID.AttackDecrease},
                {ModifierID.AttackMax, itemCollection.playerData.attack += (float)ModifierID.AttackMax},
                {ModifierID.DefenseIncrease, itemCollection.playerData.defense += (float)ModifierID.DefenseIncrease},
                {ModifierID.DefenseDecrease, itemCollection.playerData.defense += (float)ModifierID.DefenseDecrease },
                {ModifierID.DefenseMax, itemCollection.playerData.defense += (float)ModifierID.DefenseMax},
                {ModifierID.SpeedIncrease, itemCollection.playerData.speed += (float)ModifierID.SpeedIncrease},
                {ModifierID.SpeedDecrease, itemCollection.playerData.speed -= (float)ModifierID.SpeedDecrease},
                {ModifierID.SpeedMax, itemCollection.playerData.speed += (float)ModifierID.SpeedMax},
            };

        return Value[modID];
    }
    public void SpawnNewItem(Item item, InventorySlot slot)
    {
        GameObject go = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = go.GetComponent<InventoryItem>();
        inventoryItem.InitializeItem(item);
    }
}
