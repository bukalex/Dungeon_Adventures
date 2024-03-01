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
    public List<Vector2> items;

    public GameData()
    {
        checkpointsReached = 0;
        timers = new List<Vector2>();
        intKeyCodes = new List<int>();
        items = new List<Vector2>();
    }
}