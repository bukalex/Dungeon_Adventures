using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts.Enums;
using Assets.Scripts.InventoryElements;
using UnityEditor.Tilemaps;


public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] public TMP_Text countText;
    [SerializeField] public GameObject InventoryItemPrefab;
    [SerializeField] private int Id;
    [SerializeField] public Dictionary<int, List<ModifierID>> itemModifiers = new Dictionary<int, List<ModifierID>>();

    public Item item;
    public Image image;
    public InventorySlot currentInventorySlot;
    public string itemTag;
    public bool isLocked = false;
    public bool isUsable = false;
    public int maxStack;

    private void Update()
    {
    }
    public void InitializeItem(Item newItem)
    {
        Item item = newItem;
        image.sprite = newItem.sprite;
        Id = item.Id;
        item.inventoryItemPref = gameObject;
        if(newItem.Modifier.Count > 0)
            itemModifiers.Add(newItem.Id, newItem.Modifier);

        maxStack = newItem.InitializeStackableItem(newItem.ItemType);

        if (maxStack > 0)
        {
            isUsable = true;
            countText.enabled = true;
        }
        else
        {
            isUsable = false;
            countText.enabled = false;
        }
    }

    public int GetItemID(Item item)
    {
        return item.Id;
    }

    public List<ModifierID> GetItemModifiers(Item item)
    {
        return itemModifiers[item.Id];
    }

    public void updateCount(Item item, Item existingItem)
    {
        countText.text = (item.Count + existingItem.Count).ToString();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isLocked)
        {
            itemTag = InventoryItemPrefab.tag;
            image.raycastTarget = false;
            currentInventorySlot = GetComponentInParent<InventorySlot>();
            transform.SetParent(transform.root);
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!isLocked)
        {
            transform.position = Input.mousePosition;
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isLocked)
        {
            GameObject go = eventData.pointerEnter ??= gameObject;
            InventorySlot slot = go.GetComponent<InventorySlot>() ?? currentInventorySlot;
            transform.SetParent(slot?.transform);
            image.raycastTarget = true;
        }
    }
}
