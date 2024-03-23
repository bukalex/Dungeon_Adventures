using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Level Configuration", menuName = "ScriptableObjects/Level Configuration")]
public class LevelConfiguration : ScriptableObject
{
    public int enemyCounter;
    public GameObject[] spawnedEnemies;
    public GameObject[] enemies;
    public GameObject[] obstacles;
    public TileBase[] groundTiles;
    public TileBase[] wallsTiles;
}
