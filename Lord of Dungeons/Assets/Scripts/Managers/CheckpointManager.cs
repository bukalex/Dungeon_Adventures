using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class CheckpointManager : MonoBehaviour
{
    [SerializeField]
    private int checkpointPeriod = 5;
    private int checkpointsReached;
    private string filePath = "Assets/Resources/GameData.json";
    private GameData gameData = new GameData();

    public static CheckpointManager Instance { get; private set; }

    void Start()
    {
        if (Instance == null) Instance = this;
        Initialize();
    }

    private void Initialize()
    {
        checkpointsReached = 0;
        LoadData();
        UIManager.Instance.InitializeTeleportWindow(checkpointsReached, checkpointPeriod);
    }

    //Checks if we reached checkpoint
    public void ChangeLevel(int levelNumber)
    {
        if (levelNumber % checkpointPeriod == 0)
        {
            checkpointsReached++;
            UIManager.Instance.UpdateTeleportWindow(checkpointsReached, checkpointPeriod);
        }
        SaveData();
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
            for (int i = 0; i < InventoryManager.Instance.internalInventorySlots.Length; i++)
            {
                InventoryManager.Instance.LoadItem(InventoryManager.Instance.internalInventorySlots, i, (int)gameData.inventoryItems[i].x, (int)gameData.inventoryItems[i].y);
            }
            for (int i = 0; i < InventoryManager.Instance.toolBar.Length; i++)
            {
                InventoryManager.Instance.LoadItem(InventoryManager.Instance.toolBar, i, (int)gameData.toolBarItems[i].x, (int)gameData.toolBarItems[i].y);
            }
            for (int i = 0; i < InventoryManager.Instance.equipmentSlots.Length; i++)
            {
                InventoryManager.Instance.LoadItem(InventoryManager.Instance.equipmentSlots, i, (int)gameData.equipmentItems[i].x, (int)gameData.equipmentItems[i].y);
            }
        }
    }

    private void OnApplicationQuit()
    {
        SaveData();
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

    public GameData()
    {
        checkpointsReached = 0;
        timers = new List<Vector2>();
        intKeyCodes = new List<int>();
        inventoryItems = new List<Vector2>();
        toolBarItems = new List<Vector2>();
        equipmentItems = new List<Vector2>();
    }
}