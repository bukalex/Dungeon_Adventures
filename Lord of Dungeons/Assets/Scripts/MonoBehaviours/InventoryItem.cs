using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    public Item item;
    [SerializeField]
    public TMP_Text countText;
    [SerializeField]
    public GameObject InventoryItemPrefab;
    [SerializeField]
    public int craftID;

    [HideInInspector]
    public Image image;
    [HideInInspector]
    public int count = 1;
    [HideInInspector]
    public Transform parentAfterDrag;
    [HideInInspector]
    public string itemTag;
    [HideInInspector]
    public bool isLocked = false;

    public void InitializeItem(Item newItem)
    {
        item = newItem;                                             
        image.sprite = newItem.image;
        InventoryItemPrefab.tag = newItem.tag.ToString();
        craftID = newItem.craftId;
        
        updateCount();
    }
    public void updateCount()
    {
        countText.text = count.ToString();
        bool textActive = count > 1;
        countText.gameObject.SetActive(textActive);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isLocked)
        {
            itemTag = InventoryItemPrefab.tag;
            image.raycastTarget = false;
            parentAfterDrag = transform.parent;
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
            image.raycastTarget = true;
            transform.SetParent(parentAfterDrag);
        }
    }
}
