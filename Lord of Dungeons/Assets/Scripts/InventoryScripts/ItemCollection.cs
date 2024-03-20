using Assets.Scripts.InventoryElements;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "ScriptableObjects/ItemCollection")]
 public class ItemCollection : ScriptableObject
 {
     public List<Item> Items = new List<Item>();   

     [Header("Data")]
     public PlayerData playerData;
 }
