using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SOManager : MonoBehaviour
{
    [SerializeField]
    private PlayerData playerData;

    [SerializeField]
    private List<EnemyParameters> enemies;

    [SerializeField]
    private List<ItemParameters> items;

    void Awake()
    {
        playerData.SetDictionaries();

        foreach (EnemyParameters enemy in enemies)
        {
            enemy.playerData = playerData;
        }

        foreach (ItemParameters item in items)
        {
            item.playerData = playerData;
        }
    }
}
