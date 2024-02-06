using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "New Object Parameters", menuName = "ScriptableObjects/Object parameters")]

public class ObjectParameters : ScriptableObject
{
    public PlayerData playerData;
    public RuntimeAnimatorController animController;
    public GameObject dropPrefab;

    [Header("Stats")]
    public float colliderRadius = 1.0f;
    public float health = 30.0f;
    public float healthRestoreRate = 1.0f;
    public float attack = 3.0f;
    public float defense = 2.0f;
    public float specialDefense = 2.0f;


    [Header("Attributes")]

    public bool isInteractable;
    public ObjectType objectType;
    public enum ObjectType {Interactable, Breakable, Collectable, NONE, POT, CHEST, PORTAL}


}
