using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Recipes;
using Material = Assets.Scripts.Recipes.Material;
using static UnityEditor.Progress;

[CreateAssetMenu(menuName = "ScriptableObjects/RecipeCollection")]
public class RecipeCollection : ScriptableObject
{
    public List<ItemRecipe> Recipe = new List<ItemRecipe>();

    public List<Material> materials = new List<Material>();

    public Item GetCraftableItem(int itemId)
    {
        return Recipe[itemId].craftItem;
    }
    public List<ItemRecipe> GetListOfRecipes()
    {
        return Recipe;
    }
    public List<MaterialToCraft> GetMaterialsToCraft(int itemId)
    {
        return Recipe[itemId].materials;
    }
    public Sprite GetMaterialSpriteByMaterialId(int materialID)
    {
        if (materials.Contains(materials[materialID]))
        {
            return materials[materialID].MaterialSprite;
        }
        return null;
    }

    public Sprite[] GetMaterialsSpritesByItemID(int ItemID, RecipeCollection recipe)
    {
        List<MaterialToCraft> materialToCrafts = recipe.GetMaterialsToCraft(ItemID);
        int[] materialIDs = new int[materialToCrafts.Count];
        for (int i = 0; i < materialToCrafts.Count; i++)
        {
            materialIDs[i] = materialToCrafts[i].materialId;
        }

        List<Sprite> sprites = new List<Sprite>();
        foreach (int materialID in materialIDs)
        {
            sprites.Add(GetMaterialSpriteByMaterialId(materialID));
        }
        return sprites.ToArray();
    }
    public int[] GetMaterialAmountsToCraftByItemID(int ItemID, RecipeCollection recipe)
    {
        List<MaterialToCraft> materialToCrafts = recipe.GetMaterialsToCraft(ItemID);

        List<int> materialAmounts = new List<int>();
        for(int i = 0;i < materialToCrafts.Count;i++)
        {
            materialAmounts.Add(materialToCrafts[i].amount);
        }
        return materialAmounts.ToArray();
    }
}
