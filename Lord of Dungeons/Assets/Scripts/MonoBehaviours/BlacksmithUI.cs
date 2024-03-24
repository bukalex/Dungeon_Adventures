using Assets.Scripts.Recipes;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlacksmithUI : Timer//, IPointerDownHandler, IPointerUpHandler
{
    public Button craftItemButton;
    public BlacksmithItemHolder currentItemHodler;
    public Text buttonText;
    public int currentItemID;
    public string itemName;
    public GameObject craftedItemSlot;

    [SerializeField] private GameObject craftedItemDisplay;
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
    private bool slotActivated = false;

    private int firstMaterialPos = 0;
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
        base.TimerUpdate();

        if (timer > 0f)
        {
            craftItemButton.GetComponentInChildren<Text>().text = timer.ToString("F1");
        }
        if (craftedItemSlot.GetComponent<InventorySlot>().GetComponentInChildren<InventoryItem>() == null)
        {
            craftItemButton.gameObject.SetActive(true);
            craftedItemDisplay.SetActive(false);
        }
        if (!craftedItemDisplay.activeSelf && slotActivated)
        {
            slotActivated = false;
            craftItemButton.GetComponentInChildren<Text>().text = "Craft";
            for (int j = 0; j < InventoryManager.Instance.internalInventorySlots.Length; j++)
            {
                InventorySlot internalSlots = InventoryManager.Instance.internalInventorySlots[j];
                InventoryItem internalItemInSlot = internalSlots.GetComponentInChildren<InventoryItem>();
                InventoryItem craftedItemInSlot = craftedItemSlot.GetComponentInChildren<InventoryItem>();
                if (internalItemInSlot == null)
                {
                    craftedItemInSlot?.transform.SetParent(internalSlots.transform);
                }
            }
            for (int i = 0; i < InventoryManager.Instance.toolBar.Length; i++)
            {
                InventorySlot slot = InventoryManager.Instance.toolBar[i];
                InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
                if (itemInSlot == null)
                {
                    craftedItemSlot.GetComponentInChildren<InventoryItem>()?.transform.SetParent(slot.transform);
                }
            }

        }

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
            if (materialInSlot != null)
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
        if (materialSlots[0].transform.childCount == 0)
        {
            foreach (BlacksmithItemHolder itemHolder in ItemHolders)
                ItemHoldersSorter(itemHolder);
        }
        foreach (KeyValuePair<int, int> pair in insertedMaterial)
        {
            if (previousFrame.Count != insertedMaterial.Count || !insertedMaterial.ContainsKey(pair.Key) || insertedMaterial[pair.Key] != previousFrame[pair.Key])
            {
                //Instantiate all item holders
                #region
                //Delete all previous item holders
                foreach (BlacksmithItemHolder itemHolder in ItemHolders)
                    Destroy(itemHolder.gameObject);
                ItemHolders.Clear();
                //Instantiate new item holders
                foreach (ItemRecipe recipe in recipe.GetListOfRecipes())
                    InitializeItemHolder(recipe.ItemId);
                #endregion
                //Sort item holders
                #region
                //Reset all item holders positions
                firstMaterialPos = 0;
                secondMaterialsPos = 0;
                thirdMaterialsPos = 0;
                //Sort item holders
                foreach (BlacksmithItemHolder itemHolder in ItemHolders)
                    ItemHoldersSorter(itemHolder);
                #endregion
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
            if (material != null)
                materials.Add(material);
        }
        #endregion

        //Get item that player wants to craft
        #region
        int craftRate = 0;
        Item craftableItem = recipe.GetCraftableItem(ItemID);
        List<MaterialToCraft> requireMaterials = recipe.GetMaterialsToCraft(ItemID);
        if (insertedMaterial.Count == requireMaterials.Count)
        {
            //if inserted material mataches required materials, 
            foreach (MaterialToCraft requiredMaterial in requireMaterials)
                if (insertedMaterial.ContainsKey(requiredMaterial.materialId) && (insertedMaterial[requiredMaterial.materialId] >= requiredMaterial.amount))
                    craftRate++;

            Debug.Log(craftRate);
            if (craftRate == requireMaterials.Count)
            {
                SetTimer(5f, () =>
                {
                    craftItemButton.gameObject.SetActive(false);
                    craftedItemDisplay.SetActive(true);
                    InventoryManager.Instance.spawnNewItem(craftableItem, craftedItemSlot.GetComponent<InventorySlot>());
                    slotActivated = true;
                    if (TrainingManager.Instance != null) TrainingManager.Instance.itemPurchased = true;
                });
                for (int i = 0; i < materialSlots.Length; i++)
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
        List<MaterialToCraft> materials = recipe.GetMaterialsToCraft(ItemID);
        for (int j = 0; j < materials.Count; j++)
        {
            if (insertedMaterial.ContainsKey(materials[j].materialId))
            {
                GameObject newItemHolder = Instantiate(ItemHolderPrefab, ContainerOfItemHolders.transform);
                //Check if item holder already exist
                #region
                BlacksmithItemHolder itemHolder = newItemHolder.GetComponent<BlacksmithItemHolder>();
                BlacksmithItemHolder exisitingItemHolder = ItemHolders.Find(itemHolder => itemHolder.ItemID == ItemID);
                if (exisitingItemHolder == null)
                {
                    itemHolder.ItemID = ItemID;
                    ItemHolders.Add(itemHolder);
                }
                else
                {
                    Destroy(newItemHolder);
                    break;
                }
                #endregion

                //Initialize item holder with an item
                Sprite[] materialSprites = recipe.GetMaterialsSpritesByItemID(ItemID, recipe);
                int[] materialAmounts = recipe.GetMaterialAmounts(ItemID, recipe).ToArray();

                //Initialize Item Icon that it will craft
                itemHolder.itemIcon.GetComponent<Image>().sprite = recipe.GetListOfRecipes()[ItemID].craftItem.image;

                //Initialize material icons that we need to craft an item
                for (int i = 0; i < materialSprites.Length; i++)
                {
                    itemHolder.materialDisplays[i].transform.GetChild(1).GetComponent<Image>().sprite = materialSprites[i];
                    itemHolder.materialDisplays[i].transform.GetChild(1).GetComponent<Image>().color = new Color(255f, 255f, 255f, 255f);
                }

                //Initialize amount of material that we need to craft an item
                for (int i = 0; i < materialAmounts.Length; i++)
                {
                    itemHolder.materialDisplays[i].transform.GetChild(0).GetComponent<TMP_Text>().text = materialAmounts[i].ToString();
                    itemHolder.materialDisplays[i].transform.GetChild(0).GetComponent<TMP_Text>().color = Color.red;
                }
                break;
            }
        }

    }
    public void ItemHoldersSorter(BlacksmithItemHolder itemHolder)
    {
        int twoMaterialsPos = firstMaterialPos + secondMaterialsPos;
        int threeMaterialsPos = firstMaterialPos + twoMaterialsPos + thirdMaterialsPos;
        List<MaterialToCraft> requireMaterials = recipe.GetMaterialsToCraft(itemHolder.ItemID);

        /*        for (int i = 0; i < requireMaterials.Count; i++)
                {
                    InventorySlot materialSlot = materialSlots[i] ??= null;
                    InventoryItem material = materialSlot.GetComponentInChildren<InventoryItem>();
                    if (insertedMaterial.ContainsKey(requireMaterials[i].materialId) && material.count == int.Parse(currentItemHodler.materialDisplays[j].transform.GetChild(0).GetComponent<TMP_Text>().text) && insertedMaterial.ContainsKey(material.materialID))
                        itemHolder.materialDisplays[i].transform.GetChild(0).GetComponent<TMP_Text>().color = Color.green;
                    else
                        itemHolder.materialDisplays[i].transform.GetChild(0).GetComponent<TMP_Text>().color = Color.red;
                }*/

        /*        for (int i = 0; i < recipe.Recipe.Count; i++)
                {
                    for (int n = 0; n < materialSlots.Length; n++)
                    {
                        if (requireMaterials.Count <= 2)
                            if (n == 3) break;
                        InventorySlot materialSlot = materialSlots[n] ??= null;
                        InventoryItem material = materialSlot.GetComponentInChildren<InventoryItem>();

                        for (int j = 0; j < currentItemHodler.materialDisplays.Length; j++)
                            if (material.image.sprite == currentItemHodler.materialDisplays[j].transform?.GetChild(1).GetComponent<Image>().sprite)
                                if (material.count == int.Parse(currentItemHodler.materialDisplays[j].transform.GetChild(0).GetComponent<TMP_Text>().text) && insertedMaterial.ContainsKey(material.materialID))
                                    itemHolder.materialDisplays[j].transform.GetChild(0).GetComponent<TMP_Text>().color = Color.green;

                        if (material.count == 0)
                            Destroy(material.gameObject);
                    }
                }*/

        foreach (BlacksmithItemHolder itemHold in ItemHolders)
        {
            for (int i = 0; i < materialSlots.Length; i++)
            {
                InventorySlot materialSlot = materialSlots[i]; Debug.Log(materialSlot.ToString());

                InventoryItem material = materialSlot.GetComponentInChildren<InventoryItem>();
                if (material == null)
                {
                    Debug.Log("null");
                    break;
                }

                for (int j = 0; j < 4; j++)
                {
                    if (material.image.sprite == itemHold.materialDisplays[j].transform.GetChild(1).GetComponent<Image>().sprite &&
                        material.count >= int.Parse(itemHold.materialDisplays[j].transform.GetChild(0).GetComponent<TMP_Text>().text))
                    {
                        itemHold.materialDisplays[j].transform.GetChild(0).GetComponent<TMP_Text>().color = Color.green;
                        break;
                    }
                }


            }
        }
        if (requireMaterials.Count == 1)
        {
            if (itemHolder.materialDisplays[0].transform.GetChild(0).GetComponent<TMP_Text>().color == Color.red)
                firstMaterialPos--;

            itemHolder.ChildPos = firstMaterialPos;
            firstMaterialPos++;
        }
        else if (requireMaterials.Count == 2)
        {
            secondMaterialsPos++;
            itemHolder.ChildPos = twoMaterialsPos;
        }
        else if (requireMaterials.Count == 3)
        {
            thirdMaterialsPos += 2;
            itemHolder.ChildPos = threeMaterialsPos;
        }
    }
}
