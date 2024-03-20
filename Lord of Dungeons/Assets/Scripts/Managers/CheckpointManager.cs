using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    [SerializeField]
    private TemporaryTradingSystem tradingSystem;

    [SerializeField]
    private int checkpointPeriod = 5;
    private int checkpointsReached;
    public int levelsPassed = 0;
    private string filePath = "Assets/Resources/GameData.json";
    private GameData gameData = new GameData();
    public List<int> commonLevels = new List<int>();
    public List<int> bossLevels = new List<int>();
    public List<int> checkpoints = new List<int>();

    public static CheckpointManager Instance { get; private set; }

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            Initialize();
        }
    }

    private void Initialize()
    {
        for (int i = 3; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            if ((i - 3) % 5 < 3) commonLevels.Add(i);
            if ((i - 3) % 5 == 3) bossLevels.Add(i);
            if ((i - 3) % 5 == 4) checkpoints.Add(i);
        }
        checkpointsReached = 0;
        LoadData();
        UIManager.Instance.InitializeTeleportWindow(checkpointsReached, checkpointPeriod);
    }

    //Checks if we reached checkpoint
    public void ChangeLevel()
    {
        levelsPassed++;
        if (levelsPassed % checkpointPeriod == 0)
        {
            if (levelsPassed / checkpointPeriod > checkpointsReached)
            {
                checkpointsReached++;
                UIManager.Instance.UpdateTeleportWindow(checkpointsReached, checkpointPeriod);
            }
        }
        SaveData();
    }

    private int FindByName(Ability[] abilities, string abilityName)
    {
        for (int i = 0; i < abilities.Length; i++)
        {
            if (abilities[i].abilityName.Equals(abilityName)) return i;
        }
        return -1;
    }

    public void SaveData()
    {
        gameData.checkpointsReached = checkpointsReached;
        gameData.timers = DataManager.Instance.GetNPCTimers();
        List<int> intKeyCodes = new List<int>();
        foreach (KeyCode keyCode in UIManager.Instance.keyCodes)
        {
            intKeyCodes.Add((int)keyCode);
        }
        gameData.intKeyCodes = intKeyCodes;
        List<Vector2> inventoryItems = new List<Vector2>();
        foreach (InventorySlot slot in InventoryManager.Instance.internalInventorySlots)
        {
            if (slot.GetComponentInChildren<InventoryItem>() != null)
            {
                inventoryItems.Add(new Vector2(Array.IndexOf(InventoryManager.Instance.allItems, slot.GetComponentInChildren<InventoryItem>().item), slot.GetComponentInChildren<InventoryItem>().count));
            }
            else
            {
                inventoryItems.Add(new Vector2(-1, 0));
            }
        }
        gameData.inventoryItems = inventoryItems;
        List<Vector2> toolBarItems = new List<Vector2>();
        foreach (InventorySlot slot in InventoryManager.Instance.toolBar)
        {
            if (slot.GetComponentInChildren<InventoryItem>() != null)
            {
                toolBarItems.Add(new Vector2(Array.IndexOf(InventoryManager.Instance.allItems, slot.GetComponentInChildren<InventoryItem>().item), slot.GetComponentInChildren<InventoryItem>().count));
            }
            else
            {
                toolBarItems.Add(new Vector2(-1, 0));
            }
        }
        gameData.toolBarItems = toolBarItems;
        List<Vector2> equipmentItems = new List<Vector2>();
        foreach (InventorySlot slot in InventoryManager.Instance.equipmentSlots)
        {
            if (slot.GetComponentInChildren<InventoryItem>() != null)
            {
                equipmentItems.Add(new Vector2(Array.IndexOf(InventoryManager.Instance.allItems, slot.GetComponentInChildren<InventoryItem>().item), slot.GetComponentInChildren<InventoryItem>().count));
            }
            else
            {
                equipmentItems.Add(new Vector2(-1, 0));
            }
        }
        gameData.equipmentItems = equipmentItems;
        gameData.vaultCapacity = UIManager.Instance.bankerWindow.GetComponent<BankerWindow>().vaultCapacity;
        List<Vector2> bankerItems = new List<Vector2>();
        foreach (InventorySlot slot in UIManager.Instance.bankerWindow.GetComponent<BankerWindow>().vault.GetComponentsInChildren<InventorySlot>())
        {
            if (slot.GetComponentInChildren<InventoryItem>() != null)
            {
                bankerItems.Add(new Vector2(Array.IndexOf(InventoryManager.Instance.allItems, slot.GetComponentInChildren<InventoryItem>().item), slot.GetComponentInChildren<InventoryItem>().count));
            }
            else
            {
                bankerItems.Add(new Vector2(-1, 0));
            }
        }
        gameData.bankerItems = bankerItems;
        gameData.gold = DataManager.Instance.playerData.resources[Item.CoinType.GoldenCoin];
        gameData.silver = DataManager.Instance.playerData.resources[Item.CoinType.SilverCoin];
        gameData.copper = DataManager.Instance.playerData.resources[Item.CoinType.CopperCoin];
        List<Vector2> inventoryAbilities = new List<Vector2>();
        foreach (AbilitySlot slot in InventoryManager.Instance.abilityInventory)
        {
            if (slot.GetComponentInChildren<AbilityItem>() != null)
            {
                inventoryAbilities.Add(new Vector2(FindByName(InventoryManager.Instance.allAbilities, slot.GetComponentInChildren<AbilityItem>().ability.abilityName), slot.GetComponentInChildren<AbilityItem>().ability.attackParameters.rank));
            }
            else
            {
                inventoryAbilities.Add(new Vector2(-1, 1));
            }
        }
        gameData.inventoryAbilities = inventoryAbilities;
        List<Vector2> toolBarAbilities = new List<Vector2>();
        foreach (AbilitySlot slot in InventoryManager.Instance.abilityBar)
        {
            if (slot.GetComponentInChildren<AbilityItem>() != null)
            {
                toolBarAbilities.Add(new Vector2(FindByName(InventoryManager.Instance.allAbilities, slot.GetComponentInChildren<AbilityItem>().ability.abilityName), slot.GetComponentInChildren<AbilityItem>().ability.attackParameters.rank));
            }
            else
            {
                toolBarAbilities.Add(new Vector2(-1, 1));
            }
        }
        gameData.toolBarAbilities = toolBarAbilities;
        gameData.wizardLuck = tradingSystem.wizardLuck;

        string jsonData = JsonUtility.ToJson(gameData);
        File.WriteAllText(filePath, jsonData);
    }

    public void LoadData()
    {
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);

            gameData = JsonUtility.FromJson<GameData>(jsonData);
            checkpointsReached = gameData.checkpointsReached;
            DataManager.Instance.SetNPCTimers(gameData.timers);
            for (int i = 0; i < gameData.intKeyCodes.Count; i++)
            {
                UIManager.Instance.keyCodes[i] = Enum.Parse<KeyCode>(Enum.GetName(typeof(KeyCode), gameData.intKeyCodes[i]));
            }
            UIManager.Instance.InitializeKeyCodeSettings();
            InventoryManager.Instance.InitializeSlots();
            for (int i = 0; i < gameData.inventoryItems.Count; i++)
            {
                InventoryManager.Instance.LoadItem(InventoryManager.Instance.internalInventorySlots, i, (int)gameData.inventoryItems[i].x, (int)gameData.inventoryItems[i].y);
            }
            for (int i = 0; i < gameData.toolBarItems.Count; i++)
            {
                InventoryManager.Instance.LoadItem(InventoryManager.Instance.toolBar, i, (int)gameData.toolBarItems[i].x, (int)gameData.toolBarItems[i].y);
            }
            for (int i = 0; i < gameData.equipmentItems.Count; i++)
            {
                InventoryManager.Instance.LoadItem(InventoryManager.Instance.equipmentSlots, i, (int)gameData.equipmentItems[i].x, (int)gameData.equipmentItems[i].y);
            }
            UIManager.Instance.bankerWindow.GetComponent<BankerWindow>().LoadVault(gameData.vaultCapacity, gameData.bankerItems);
            DataManager.Instance.playerData.resources[Item.CoinType.GoldenCoin] = gameData.gold;
            DataManager.Instance.playerData.resources[Item.CoinType.SilverCoin] = gameData.silver;
            DataManager.Instance.playerData.resources[Item.CoinType.CopperCoin] = gameData.copper;
            InventoryManager.Instance.LoadAbilityInventory(gameData.inventoryAbilities.Count);
            for (int i = 0; i < gameData.inventoryAbilities.Count; i++)
            {
                InventoryManager.Instance.LoadAbility(InventoryManager.Instance.abilityInventory, i, (int)gameData.inventoryAbilities[i].x, (int)gameData.inventoryAbilities[i].y);
            }
            for (int i = 0; i < gameData.toolBarAbilities.Count; i++)
            {
                InventoryManager.Instance.LoadAbility(InventoryManager.Instance.abilityBar, i, (int)gameData.toolBarAbilities[i].x, (int)gameData.toolBarAbilities[i].y);
            }
            tradingSystem.wizardLuck = gameData.wizardLuck;
        }
    }

    private void OnApplicationQuit()
    {
        if (!DataManager.Instance.isEducating) SaveData();
    }
}

[System.Serializable]
public class GameData
{
    public int checkpointsReached;
    public List<Vector2> timers;
    public List<int> intKeyCodes;
    public List<Vector2> inventoryItems;
    public List<Vector2> toolBarItems;
    public List<Vector2> equipmentItems;
    public int vaultCapacity;
    public List<Vector2> bankerItems;
    public int gold;
    public int silver;
    public int copper;
    public List<Vector2> inventoryAbilities;
    public List<Vector2> toolBarAbilities;
    public float wizardLuck;

    public GameData()
    {
        checkpointsReached = 0;
        timers = new List<Vector2>();
        intKeyCodes = new List<int>();
        inventoryItems = new List<Vector2>();
        toolBarItems = new List<Vector2>();
        equipmentItems = new List<Vector2>();
        vaultCapacity = 0;
        bankerItems = new List<Vector2>();
        gold = 0;
        silver = 0;
        copper = 0;
        inventoryAbilities = new List<Vector2>();
        toolBarAbilities = new List<Vector2>();
        wizardLuck = 100;
    }
}