using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            item.playerData = playerData;   
        }

        foreach (NPCParameters npc in npcs)
        {
            npc.playerData = playerData;
        }

        if (inventory != null)
        {
            inventory.playerData = playerData;
        }
        if (UI != null)
        {
            UI.playerData = playerData; 
        }
    }

    private void Update()
    {
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
}
