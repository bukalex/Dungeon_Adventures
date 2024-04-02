using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Recipes
{
    [Serializable]
    public struct ItemRecipe
    {
        public Item craftItem;
        [Tooltip("Item ID assigns based on index of an element in the list \"Recipe\"")]
        public int ItemId;
        public string ItemName { get; private set; }
        public Sprite ItemIcon { get; private set; }
        public List<MaterialToCraft> materials;
    }
    [Serializable]
    public struct MaterialToCraft
    {
        public int amount;
        public int materialId;
    }
    [Serializable]
    public struct Material
    {
        public Sprite MaterialSprite;
        public int MaterialId;
        public string MaterialName;

        public Sprite GetMaterialSprite(int materialID)
        {
            return MaterialSprite;
        }
        public string GetMAttribute(Item item)
        {
            return MaterialName = item.name;
        }
    }
}
