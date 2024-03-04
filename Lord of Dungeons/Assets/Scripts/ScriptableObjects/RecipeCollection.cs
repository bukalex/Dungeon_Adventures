using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Recipes;
using Material = Assets.Scripts.Recipes.Material;

[CreateAssetMenu(menuName = "ScriptableObjects/RecipeCollection")]
public class RecipeCollection : ScriptableObject
{
    public List<ItemRecipe> Recipe = new List<ItemRecipe>();

    public List<Material> materials = new List<Material>();

    public Item GetCraftableItem(int itemId)
    {
        return Recipe[itemId].craftItem;
    }
    public List<MaterialToCraft> GetMaterialsToCraft(int itemId)
    {
        return Recipe[itemId].materials;
    }
}
