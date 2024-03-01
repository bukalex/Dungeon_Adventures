using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Enums;
using System;
using Assets.Scripts.Enums.ItemType;

namespace Assets.Scripts.InventoryElements
{
    [Serializable]
    public class Item
    {
        public string name;
        public int Id;
        public int Count;
        public Sprite sprite;
        public ItemType ItemType;
        public List<ModifierID> Modifier;
        
        public Item(int id, ItemType itemType)
        {
            Id = id;
            ItemType = itemType;
        }

        public Item(int id, int count, ItemType itemType)
        {
            Id = id;
            Count = count;
            ItemType = itemType;
        }
        public Item(int id, int count, ItemType itemType, List<ModifierID> modifier)
        {
            Id = id;
            Count = count;
            ItemType = itemType;
            Modifier = modifier;
        }

        public Item Clone()
        {
            return (Item) MemberwiseClone();
        }
    }
}