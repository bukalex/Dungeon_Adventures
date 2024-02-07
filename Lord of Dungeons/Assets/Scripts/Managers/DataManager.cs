using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{

    [SerializeField]
    private PlayerData playerData;

    [SerializeField]
    private List<EnemyParameters> enemies;

    [SerializeField]
    private List<Item> items;

    [SerializeField]
    private List<NPCParameters> npcs;

    [SerializeField]
    private TemporaryTradingSystem tradingSystem;

    [SerializeField]
    private InventoryManager inventory;

    void Awake()
    {
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

        if (tradingSystem != null)
        {
            tradingSystem.playerData = playerData;
        }
        if (inventory != null)
        {
            inventory.playerData = playerData;
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
