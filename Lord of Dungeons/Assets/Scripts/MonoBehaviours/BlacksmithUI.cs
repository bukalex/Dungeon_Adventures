using Assets.Scripts.Recipes;
using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private GameObject materiaSlotsGroup;
    [SerializeField] private InventorySlot[] materialSlots;
    [SerializeField] private GameObject ItemHolderPrefab;
    [SerializeField] private RecipeCollection recipe;
    [SerializeField] private Button CraftItemButton;
    [SerializeField] private GameObject materialRequireDisplayPrefab;
    [SerializeField] private GameObject materialDisplayPrefab;
    [SerializeField] private GameObject materialIconPrefab;
    [SerializeField] private TMP_Text materialAmount;
    private Dictionary<int, int> insertedMaterial = new Dictionary<int, int>();
    private Dictionary<int, int> previousFrame = new Dictionary<int, int>();

    private void Awake()
    {
        materialSlots = materiaSlotsGroup.GetComponentsInChildren<InventorySlot>();

    }
    private void Update()
    {

        if(Input.GetKey(KeyCode.Escape)) 
            Debug.Log(insertedMaterial);
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
    public void InstantiateNewItemHolder(int recipeID)
    {

    }
    public void InitializeItemIcon(Sprite sprite)
    {

    }

    public void InitializeMaterialDisplay(GameObject itemHolderPref)
    {
        GameObject materialsRequireDisplay = Instantiate(materialRequireDisplayPrefab, itemHolderPref.transform.parent);
        List<MaterialToCraft> requireMaterials = recipe.GetMaterialsToCraft(0);
        List<GameObject> materialDisplays = new List<GameObject>();
        List<TMP_Text> costs = new List<TMP_Text>();
        List<GameObject> materialIcons = new List<GameObject>();
        for (int i = 0; i < requireMaterials.Count; i++)
        {
            GameObject newMaterialDisplay = Instantiate(materialDisplayPrefab, materialRequireDisplayPrefab.transform.parent);
            materialDisplays.Add(newMaterialDisplay);
        }
        foreach (GameObject materialDisplay in materialDisplays)
        {
            TMP_Text cost = Instantiate(materialAmount, materialDisplay.transform.parent);
            costs.Add(cost);
            GameObject materialIcon = Instantiate(materialIconPrefab, materialDisplay.transform.parent);
            materialIcons.Add(materialIcon);
        }
        foreach(MaterialToCraft material in requireMaterials)
        {
            foreach (GameObject materialIcon in materialIcons)
                materialIcon.GetComponent<Image>().sprite = recipe.GetMaterialSprite(material.materialId);
        }
    }
}
