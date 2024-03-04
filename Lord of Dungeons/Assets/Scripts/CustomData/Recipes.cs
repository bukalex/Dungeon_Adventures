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

        public Sprite GetMaterialSprite(Item item)
        {
            return MaterialSprite = item.image;
        }
        public string GetMAttribute(Item item)
        {
            return MaterialName = item.name;
        }
    }
}
