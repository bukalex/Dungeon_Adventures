using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelConfiguration levelConfiguration;
    [HideInInspector] public GameObject[] spawnedEnemies;

    public static LevelManager Instance { get; private set;}

    // Update is called once per frame
    void Update()
    {

        GameObject[] spawnedEnemies = levelConfiguration.spawnedEnemies;
        spawnedEnemies = GameObject.FindGameObjectsWithTag("Enemy");
    }
}
