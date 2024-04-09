using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Recipes;
using Material = Assets.Scripts.Recipes.Material;

[CreateAssetMenu(menuName = "ScriptableObjects/RecipeCollection")]
public class RecipeCollection : ScriptableObject
{
    public List<DishRecipe> Dishes = new List<DishRecipe>();
    public List<ItemRecipe> Recipe = new List<ItemRecipe>();

    public List<Material> materials = new List<Material>();

    public List<Item> GetIngridientList(DishRecipe recipe)
    {
        List<Item> ingridientList = new List<Item>();

        foreach (RequiredIngridients ingridient in recipe.ingridients)
        {
            ingridientList.Add(ingridient.ingridient);
        }

        return ingridientList;
    }
    public string GetItemName(int ItemID)
    {
        return Recipe[ItemID].ItemName;
    }
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
    public int[] GetMaterialAmounts(int ItemID, RecipeCollection recipe)
    {
        List<MaterialToCraft> materialToCrafts = recipe.GetMaterialsToCraft(ItemID);

        List<int> materialAmounts = new List<int>();
        for(int i = 0;i < materialToCrafts.Count;i++)
        {
            materialAmounts.Add(materialToCrafts[i].amount);

        }
        return materialAmounts.ToArray();
    }
    //public int GetMaterialAmount(int itemID, RecipeCollection recipe)
    //{
    //    List<MaterialToCraft> materialToCrafts = recipe.GetMaterialsToCraft(itemID);
    //    return materialToCrafts[itemID].amount;
    //}
}
