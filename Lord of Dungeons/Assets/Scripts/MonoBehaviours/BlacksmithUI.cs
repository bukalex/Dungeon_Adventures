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
using static UnityEditor.Progress;
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
    [SerializeField] private List<BlacksmithItemHolder> ItemHolders;

    //Key is material ID, Value is material amount to craft
    private Dictionary<int, int> insertedMaterial = new Dictionary<int, int>();
    private Dictionary<int, int> previousFrame = new Dictionary<int, int>();
    private int recipeAmount;
    private bool filled = true;

    private int firstMaterialPos= 0;
    private int secondMaterialsPos = 0;
    private int thirdMaterialsPos = 0;
    public static BlacksmithUI Instance { get; private set; }
    private void Awake()
    {

        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        materialSlots = materiaSlotsGroup.GetComponentsInChildren<InventorySlot>(true);
    }
    private void OnEnable()
    {
        UIManager.Instance.InventorySlots.SetActive(true);
        UIManager.Instance.npcWindowActive = true;
    }
    private void OnDisable()
    {
        UIManager.Instance.InventorySlots.SetActive(false);
        UIManager.Instance.npcWindowActive = false;
    }
    private void Update()
    {
        //Instantiate all item holders based on recipe amount
        #region
        if (filled)
        {
            foreach (ItemRecipe recipe in recipe.GetListOfRecipes())
                InitializeItemHolder(recipe.ItemId);
            if(ItemHolders.Count == recipe.GetListOfRecipes().Count)
                filled = false;
        }
        #endregion

        //Turn off material slot if there is no child
        #region
        for (int i = 0; i < materialSlots.Length - 1; i++)
        {
            if (materialSlots[i].transform.childCount == 1)
            {
                materialSlots[i + 1].gameObject.SetActive(true);
            }
                

            if (materialSlots[i].transform.childCount == 0)
            {
                materialSlots[i + 1].gameObject.SetActive(false);
                if (materialSlots[i + 1].transform.childCount == 1) materialSlots[i + 1].transform.GetChild(0).transform.parent = materialSlots[i].transform;   
            }
        }
            
        #endregion

        //Save previous inserted materials
        #region
        previousFrame.Clear();
        foreach (KeyValuePair<int, int> pair in insertedMaterial)
        {
            previousFrame.Add(pair.Key, pair.Value);
        }
        #endregion

        //Initialize inserted materials from each material slot
        #region
        insertedMaterial.Clear();
        for (int i = 0; i < materialSlots.Length; i++) 
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
        #endregion

        //Check every frame if something has changed in material slots
        #region
        foreach (KeyValuePair<int, int> pair in insertedMaterial)
        {
            if (previousFrame.Count != insertedMaterial.Count || !insertedMaterial.ContainsKey(pair.Key) || insertedMaterial[pair.Key] != previousFrame[pair.Key])
            {
                firstMaterialPos = 0;
                secondMaterialsPos = 0;
                thirdMaterialsPos = 0;

                foreach (BlacksmithItemHolder itemHolder in ItemHolders)
                    ItemHoldersSorter(itemHolder);

                craftItemButton.onClick.RemoveAllListeners();
                craftItemButton.onClick.AddListener(() => CraftItem(currentItemID));
                break;
            }
        }
        #endregion
    }
    public void CraftItem(int ItemID)
    {
        //Initialize each inserted material to the list of materials
        #region
        List<InventoryItem> materials = new List<InventoryItem>();
        foreach (InventorySlot materialSlot in materialSlots)
        {
            
            InventoryItem material = materialSlot.GetComponentInChildren<InventoryItem>();
            if(material != null)
                materials.Add(material);
        }
        #endregion

        //Get item that player wants to craft
        #region
        int craftRate = 0;
        Item craftableItem = recipe.GetCraftableItem(ItemID);
        List<MaterialToCraft> requireMaterials= recipe.GetMaterialsToCraft(ItemID);
        if(insertedMaterial.Count == requireMaterials.Count)
        {
            //if inserted material mataches required materials, 
            foreach (MaterialToCraft requiredMaterial in requireMaterials)
                if (insertedMaterial.ContainsKey(requiredMaterial.materialId) && (insertedMaterial[requiredMaterial.materialId] >= requiredMaterial.amount))
                    craftRate++;

            Debug.Log(craftRate);
            if (craftRate == requireMaterials.Count)
            {
                InventoryManager.Instance.AddItem(craftableItem, InventoryManager.Instance.toolBar, InventoryManager.Instance.internalInventorySlots);
                for(int i = 0; i < materialSlots.Length; i++)
                {
                    InventorySlot materialSlot = materialSlots[i];
                    InventoryItem material = materialSlot.GetComponentInChildren<InventoryItem>();

                    //List<int> materialIDs = new List<int>();
                    //for(int j = 0; j < materialSlots.Length; j++)
                    //    materialIDs.Add(material.materialID);
                    //InventoryItem material = origMaterial ??= materialSlots[materialSlots.Length - 1].GetComponentInChildren<InventoryItem>();

                    int materialCost = requireMaterials[i].amount;

                    material.count -= materialCost;
                    material.updateCount();

                    if (material.count == 0)
                        Destroy(material.gameObject);

                } 
            }
        }
        #endregion
    }

    public void InitializeItemHolder(int ItemID)
    {
        //Instantiate an item holder
        GameObject newItemHolder = Instantiate(ItemHolderPrefab, ContainerOfItemHolders.transform);

        //Initialize item holder with an item
        BlacksmithItemHolder itemHolder = newItemHolder.GetComponent<BlacksmithItemHolder>();
        ItemHolders.Add(itemHolder);
        itemHolder.ItemID = ItemID;

        Sprite[] materialSprites = recipe.GetMaterialsSpritesByItemID(ItemID, recipe);
        int[] materialAmounts = recipe.GetMaterialAmounts(ItemID, recipe).ToArray();

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
            itemHolder.materialDisplays[i].transform.GetChild(0).GetComponent<TMP_Text>().color = Color.red;
        }
    }
    public void ItemHoldersSorter(BlacksmithItemHolder itemHolder)
    {
        List<MaterialToCraft> requireMaterials = recipe.GetMaterialsToCraft(itemHolder.ItemID);

        for (int i = 0; i < requireMaterials.Count; i++)
        {
            if (insertedMaterial.ContainsKey(requireMaterials[i].materialId))
                itemHolder.materialDisplays[i].transform.GetChild(0).GetComponent<TMP_Text>().color = Color.green;
            else
                itemHolder.materialDisplays[i].transform.GetChild(0).GetComponent<TMP_Text>().color = Color.red;
        }
            

        
        if(requireMaterials.Count == 1)
        {
            
            itemHolder.ChildPos = firstMaterialPos;
            firstMaterialPos++;
        }
        else if (requireMaterials.Count == 2)
        {
            secondMaterialsPos++;
            int twoMaterialsPos = firstMaterialPos + secondMaterialsPos;
            itemHolder.ChildPos = twoMaterialsPos;
        }
        else if (requireMaterials.Count == 3)
        {
            thirdMaterialsPos++;
            int threeMaterialsPos = firstMaterialPos + secondMaterialsPos + thirdMaterialsPos;
            itemHolder.ChildPos = threeMaterialsPos;
        }
    }
}
