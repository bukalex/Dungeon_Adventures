using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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

        string jsonData = JsonUtility.ToJson(gameData);
        File.WriteAllText(filePath, jsonData);
    }

    public void LoadData()
    {
        Debug.Log("2");
        if (File.Exists(filePath))
        {
            Debug.Log("3");
            string jsonData = File.ReadAllText(filePath);
            Debug.Log(jsonData);

            gameData = JsonUtility.FromJson<GameData>(jsonData);
            checkpointsReached = gameData.checkpointsReached;
            DataManager.Instance.SetNPCTimers(gameData.timers);
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
    public List<Vector2> items;

    public GameData()
    {
        checkpointsReached = 0;
        timers = new List<Vector2>();
        items = new List<Vector2>();
    }
}