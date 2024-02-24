using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    [SerializeField]
    private int checkpointPeriod = 5;
    private int checkpointsReached;

    public static CheckpointManager Instance { get; private set; }

    void Start()
    {
        if (Instance == null) Instance = this;
        Initialize();
    }

    private void Initialize()
    {
        checkpointsReached = 0;
        //Load progress from file
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
    }
}
