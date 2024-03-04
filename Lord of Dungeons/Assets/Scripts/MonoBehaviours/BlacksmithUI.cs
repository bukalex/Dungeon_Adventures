using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlacksmithUI : MonoBehaviour
{
    public Button insertButton;

    [SerializeField] private GameObject materiaSlotsGroup;
    [SerializeField] private InventorySlot[] materialSlots;
    [SerializeField] private List<int> insertedMaterialIDs;
    [SerializeField] private List<int> insertedMaterialAmount;
    [SerializeField] private GameObject ItemHolder;
    [SerializeField] private RecipeCollection recipe;
    private Dictionary<int, int> insertedMaterial;
    

    private void Awake()
    {
        materialSlots = materiaSlotsGroup.GetComponentsInChildren<InventorySlot>();

    }

    private void Start()
    {

    }

    private void Update()
    {
        //for(int i = 0; i < materialSlots.Length; i++) 
        //    if (materialSlots[i].transform.childCount != 0)
        //        if(i == materialSlots.Length - 1)
        //            materialSlots[i].gameObject.SetActive(!materialSlots[i].gameObject.activeSelf);
        //        else
        //            materialSlots[i + 1].gameObject.SetActive(!materialSlots[i + 1].gameObject.activeSelf);

        insertButton.onClick.AddListener(() => CheckMaterialSlots());
    }

    public void InitializeItemHolder(RecipeCollection recipe)
    {
        Item craftableItem = recipe.GetCraftableItem(0);
    }

    public void InitializeItemIcon()
    {

    }

    public void InitializeMaterialDisplay()
    {

    }

    public void CheckMaterialSlots()
    {
        foreach (InventorySlot slot in materialSlots)
        {
            InventoryItem materialInSlot = slot.GetComponentInChildren<InventoryItem>();
            int craftID = materialInSlot.item.craftId;
            int materialAmount = materialInSlot.count;
            insertedMaterialIDs.Clear();

            insertedMaterial.Add(craftID, materialAmount);
        }

    }
}
