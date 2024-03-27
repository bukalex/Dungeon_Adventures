using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Battle Data", menuName = "ScriptableObjects/Battle data")]
public class BattleData : ScriptableObject
{
    public GameObject shieldPrefab;
    public Dictionary<ScriptableObject, GameObject> shieldsByCreatures = new Dictionary<ScriptableObject, GameObject>();

    public GameObject superAttackAreaPrefab;
    public GameObject boomerangPrefab;
    public Dictionary<ScriptableObject, Vector3> attackDirections = new Dictionary<ScriptableObject, Vector3>();

    public GameObject Sparks;
    public GameObject guisonKnifePrefab;
    public List<ProjectileController> guisonKnifes = new List<ProjectileController>();
    public GameObject explosionPrefab;
    public GameObject textDamagePrefab;
    public GameObject swordParticles;
    public GameObject arrowPrefab;
}
