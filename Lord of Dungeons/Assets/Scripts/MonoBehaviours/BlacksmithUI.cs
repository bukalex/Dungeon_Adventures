using Assets.Scripts.Recipes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Material = Assets.Scripts.Recipes.Material;

public class BlacksmithUI : MonoBehaviour//, IPointerDownHandler, IPointerUpHandler
{
    public Button insertButton;
    public GameObject ContainerOfItemHolders;

    [SerializeField] private GameObject materiaSlotsGroup;
    [SerializeField] private InventorySlot[] materialSlots;
    [SerializeField] private GameObject ItemHolderPrefab;
    [SerializeField] private RecipeCollection recipe;
    [SerializeField] private Button CraftItemButton;
    [SerializeField] private List<GameObject> ItemHolders;
    private Dictionary<int, int> insertedMaterial = new Dictionary<int, int>();
    private Dictionary<int, int> previousFrame = new Dictionary<int, int>();
    private int recipeAmount;
    public bool filled = true;

    private void Awake()
    {
        materialSlots = materiaSlotsGroup.GetComponentsInChildren<InventorySlot>();

    }
    private void Update()
    {
        if (filled)
        {
            foreach (ItemRecipe recipe in recipe.GetListOfRecipes())
                InitializeItemHolder(recipe.ItemId);
            if(ItemHolders.Count == recipe.GetListOfRecipes().Count)
                filled = false;
        }


        previousFrame.Clear();
        foreach(KeyValuePair<int, int> pair in insertedMaterial)
        {
            previousFrame.Add(pair.Key, pair.Value);
        }
        insertedMaterial.Clear();
        foreach (InventorySlot materialSlot in materialSlots)
        {
            InventoryItem materialInSlot = materialSlot.GetComponentInChildren<InventoryItem>();
            if(materialInSlot != null)
            {
                int materialId = materialInSlot.craftID;
                int CraftAmount = materialInSlot.count;
                if (insertedMaterial.ContainsKey(materialId))
                {
                    insertedMaterial[materialId] += CraftAmount;
                }
                else
                    insertedMaterial.Add(materialId, CraftAmount);
            }
        }
        foreach (KeyValuePair<int, int> pair in insertedMaterial)
        {
            if (previousFrame.Count != insertedMaterial.Count || !insertedMaterial.ContainsKey(pair.Key) || insertedMaterial[pair.Key] != previousFrame[pair.Key])
            {
                CraftItemButton.onClick.AddListener(() => CraftItem());

                break;
            }
        }
    }
    public void CraftItem()
    {
        int craftRate = 0;
        Item craftableItem = recipe.GetCraftableItem(0);
        List<MaterialToCraft> requireMaterials= recipe.GetMaterialsToCraft(0);
        if(insertedMaterial.Count == requireMaterials.Count)
        {
            foreach (MaterialToCraft requiredMaterial in requireMaterials)
                if (insertedMaterial.ContainsKey(requiredMaterial.materialId) && (insertedMaterial[requiredMaterial.materialId] >= requiredMaterial.amount))
                    craftRate++;

            Debug.Log(craftRate);
            if (craftRate == requireMaterials.Count)
            {
                // TODO: Remove Items when it needs, 
                InventoryManager.Instance.AddItem(craftableItem, InventoryManager.Instance.toolBar, InventoryManager.Instance.internalInventorySlots);
            }
        }
    }

    public void InitializeItemHolder(int ItemID)
    {
        GameObject newItemHolder = Instantiate(ItemHolderPrefab, ContainerOfItemHolders.transform);
        ItemHolders.Add(newItemHolder);
        BlacksmithItemHolder itemHolder = newItemHolder.GetComponent<BlacksmithItemHolder>();
        itemHolder.ItemID = ItemID;

        Dictionary<Sprite, int> materials = new Dictionary<Sprite, int>();
        Sprite[] materialSprites = recipe.GetMaterialsSpritesByItemID(ItemID, recipe);
        int[] materialAmounts = recipe.GetMaterialAmountsToCraftByItemID(ItemID, recipe);
        //Initialize Item Icon that it will craft
        itemHolder.itemIcon.GetComponent<Image>().sprite = recipe.GetListOfRecipes()[ItemID].craftItem.image;

        //Initialize material icons that we need to craft an item
        for(int i = 0; i < materialSprites.Length; i++)
        {
            itemHolder.materialDisplays[i].transform.GetChild(1).GetComponent<Image>().sprite = materialSprites[i];
            itemHolder.materialDisplays[i].transform.GetChild(1).GetComponent<Image>().color = new Color(255f, 255f, 255f, 255f);
        }

        //Initialize amount of material that we need to craft an item
        for(int i = 0; i < materialAmounts.Length; i++)
        {
            Debug.Log(materialAmounts[i].ToString());
            itemHolder.materialDisplays[i].transform.GetChild(0).GetComponent<TMP_Text>().text = materialAmounts[i].ToString();

        }
    }
}
