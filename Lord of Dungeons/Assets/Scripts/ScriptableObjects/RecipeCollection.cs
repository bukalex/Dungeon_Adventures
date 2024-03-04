using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Recipes;
using Material = Assets.Scripts.Recipes.Material;

[CreateAssetMenu(menuName = "ScriptableObjects/RecipeCollection")]
public class RecipeCollection : ScriptableObject
{
    public List<ItemRecipe> Recipes = new List<ItemRecipe>();

    public List<Material> materials = new List<Material>();
}
