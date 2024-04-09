using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Assets.Scripts.Recipes;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using System.Linq;

public class PotWindow : MonoBehaviour
{
    [SerializeField] private RecipeCollection recipeCollection;
    [SerializeField] private TMP_Text text;
    [SerializeField] private Button addIngridientButton;
    [SerializeField] private Button startCookingButton;
    [SerializeField] private InventorySlot ingridientSlot;
    [SerializeField] private InventorySlot dishSlot;
    [SerializeField] private Slider progressBar;

    private List<CustomDictionary<Item, int>> insertedIngridients = new List<CustomDictionary<Item, int>>();
    private string requiredFood;
    private int ingridientAmount;
    private void Start()
    {
        addIngridientButton.onClick.AddListener(() => InsertItem(ingridientSlot.GetComponentInChildren<InventoryItem>().item));
        startCookingButton.onClick.AddListener(() => CookDish());
    }

    private void InsertItem(Item ingridientInSlot)
    {
        if(ingridientInSlot != null)
        {
            CustomDictionary<Item, int> existedIngridient = insertedIngridients.Find(i => i.ContainsKey(ingridientInSlot));

            if(existedIngridient != null)
            {
                insertedIngridients.Find(i => i == existedIngridient)[ingridientInSlot] += ingridientSlot.GetComponentInChildren<InventoryItem>().count;
                
            }
            else
            {
                CustomDictionary<Item, int> ingrigient = new CustomDictionary<Item, int>
                {
                    { ingridientInSlot, ingridientSlot.GetComponentInChildren<InventoryItem>().count }
                };
                insertedIngridients.Insert(insertedIngridients.Count, ingrigient);
            }
            Destroy(ingridientSlot.GetComponentInChildren<InventoryItem>().gameObject);
        }
    }

    private void CookDish()
    {
        List<Item> possibleDishes = new List<Item>();

        List<Item> ingridients = new List<Item>();
        List<int> amount = new List<int>();
        
        foreach (CustomDictionary<Item, int> keys in insertedIngridients)
        {
            ingridients.Add(keys.GetKey(keys));
            amount.Add(keys.GetValueOrDefault(keys.GetKey(keys)));
        }

        foreach (DishRecipe recipe in recipeCollection.Dishes)
        {
            if (recipeCollection.GetIngridientList(recipe) == ingridients &&
                isEnoughIngridients(recipe, amount, ingridients))
            {
                possibleDishes.Add(recipe.dish);
            }
        }

        InventoryManager.Instance.spawnNewItem(possibleDishes[Random.Range(0, possibleDishes.Count - 1)]);
    }

    private bool isEnoughIngridients(DishRecipe recipe, List<int> amount, List<Item> ingridients)
    {
        List<int> ingridientAmount = new List<int>();
        int amountMatch = 0;

        foreach (RequiredIngridients ingridient in recipe.ingridients)
        {
            ingridientAmount.Add(ingridient.ingridientAmount);
        }

        for (int i = 0; i < amount.Count; i++)
        {
            int amountToCheck = amount[i];
            int match = ingridientAmount.Find(j => j == amountToCheck);

            if (match == amountToCheck)
            {
                amountMatch++;
            }
        }

        if (amountMatch == amount.Count)
        {
            return true;
        }

        return false;
    }
}
