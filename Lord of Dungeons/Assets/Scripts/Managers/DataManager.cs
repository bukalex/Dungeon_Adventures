using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts.InventoryElements;
public class DataManager : MonoBehaviour
{

    [SerializeField]
    public PlayerData playerData;

    [SerializeField]
    private List<EnemyParameters> enemies;

    [SerializeField]
    private List<Item> items;

    [SerializeField]
    private List<NPCParameters> npcs;

    [SerializeField]
    private InventoryManager inventory;
    [SerializeField]
    private LevelManager levelManager;
    [HideInInspector]
    private UIManager UI;

    public bool isEducating;

    public static DataManager Instance { get; private set; }

    void Awake()
    {
        if(Instance == null)
            Instance = this;

        playerData.SetStats();
        playerData.SetDictionaries();

        foreach (EnemyParameters enemy in enemies)
        {
            enemy.playerData = playerData;
        }

        foreach (Item item in items)
        {
            //item.playerData = playerData;   
        }

        foreach (NPCParameters npc in npcs)
        {
            npc.playerData = playerData;
        }

        if (inventory != null)
        {
            //inventory.playerData = playerData;
        }
        if (UI != null)
        {
            UI.playerData = playerData; 
        }
    }

    private void Update()
    {
        isEducating = SceneManager.GetActiveScene().name == "Education";
    }

    //Getting player position where he changed scene
    private void getPlayerPosition(Vector2 exitPosition)
    {
        exitPosition = playerData.position;
    }

    //Getting current scene 
    private void getCurrentSceneParametrs(string currentSceneName)
    {
        var activeScene = SceneManager.GetActiveScene();
        currentSceneName = activeScene.name;
    }

    public List<Vector2> GetNPCTimers()
    {
        List<Vector2> timers = new List<Vector2>();
        foreach (NPCParameters parameters in npcs)
        {
            timers.Add(new Vector2((int)parameters.type, parameters.timer));
        }

        return timers;
    }

    public void SetNPCTimers(List<Vector2> timers)
    {
        foreach (NPCParameters parameters in npcs)
        {
            foreach (Vector2 timer in timers)
            {
                if ((int)parameters.type == timer.x)
                {
                    parameters.timer = timer.y;
                    break;
                }
            }
        }
    }

    public float GetNPCTimer(int index)
    {
        return npcs[index].timer;
    }

    public void SetNPCTimer(int index, float time)
    {
        npcs[index].timer = time;
    }
}
