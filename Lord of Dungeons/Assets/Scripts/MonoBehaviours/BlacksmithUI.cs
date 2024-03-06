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
    public Button craftItemButton;
    public Text buttonText;
    public int currentItemID;
    public string itemName;

    [SerializeField] private GameObject ContainerOfItemHolders;
    [SerializeField] private GameObject materiaSlotsGroup;
    [SerializeField] private InventorySlot[] materialSlots;
    [SerializeField] private GameObject ItemHolderPrefab;
    [SerializeField] private RecipeCollection recipe;
    [SerializeField] private List<GameObject> ItemHolders;
    private Dictionary<int, int> insertedMaterial = new Dictionary<int, int>();
    private Dictionary<int, int> previousFrame = new Dictionary<int, int>();
    private int recipeAmount;
    private bool filled = true;

    public static BlacksmithUI Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

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
        for(int i = 0; i < materialSlots.Length; i++)
        {
            InventorySlot materialSlot = materialSlots[i];
            InventoryItem materialInSlot = materialSlot.GetComponentInChildren<InventoryItem>();
            if(materialInSlot != null)
            {
                int materialId = materialInSlot.materialID;
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
                Debug.Log(insertedMaterial.Count);
                craftItemButton.onClick.RemoveAllListeners();
                craftItemButton.onClick.AddListener(delegate { CraftItem(currentItemID); });
                break;
            }
        }
    }
    public void CraftItem(int ItemID)
    {
        List<InventoryItem> materials = new List<InventoryItem>();
        foreach(InventorySlot materialSlot in materialSlots)
        {
            
            InventoryItem material = materialSlot.GetComponentInChildren<InventoryItem>();
            if(material != null)
                materials.Add(material);
        }


        int craftRate = 0;
        Item craftableItem = recipe.GetCraftableItem(ItemID);
        List<MaterialToCraft> requireMaterials= recipe.GetMaterialsToCraft(ItemID);

        if(insertedMaterial.Count == requireMaterials.Count)
        {
            foreach (MaterialToCraft requiredMaterial in requireMaterials)
                if (insertedMaterial.ContainsKey(requiredMaterial.materialId) && (insertedMaterial[requiredMaterial.materialId] >= requiredMaterial.amount))
                    craftRate++;

            Debug.Log(craftRate);
            if (craftRate == requireMaterials.Count)
            {
                for(int i = 0; i < materialSlots.Length; i++)
                {
                    InventorySlot materialSlot = materialSlots[i];
                    InventoryItem material = materialSlot.GetComponentInChildren<InventoryItem>();
                    if (material != null)
                    {
                        material.count -= recipe.GetMaterialAmountsToCraftByItemID(ItemID, recipe)[i];
                        material.updateCount();
                        
                        if(material.count == 0)
                            Destroy(material.gameObject);
                    }
                }
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
            itemHolder.materialDisplays[i].transform.GetChild(0).GetComponent<TMP_Text>().text = materialAmounts[i].ToString();

        }
    }
}
