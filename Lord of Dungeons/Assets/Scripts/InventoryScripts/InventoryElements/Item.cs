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
        public GameObject inventoryItemPref;
        public Item(int id, int count)
        {
            Id = id;
            Count = count;
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

        public int InitializeStackableItem(ItemType itemType)
        {
            switch(itemType)
            {
                case ItemType.Potion: return 16;
                case ItemType.Food: return 8;
                case ItemType.MonsterLoot: return 32;
                case ItemType.Trash: return 64;
                case ItemType.UpgradeMaterial: return 16;
                default: return 0;
            }
        }
    }
}