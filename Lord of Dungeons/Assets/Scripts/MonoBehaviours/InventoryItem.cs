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

    [HideInInspector]
    public Image image;
    [HideInInspector]
    public int count = 1;
    [HideInInspector]
    public Transform parentAfterDrag;
    [HideInInspector]
    public string itemTag;

    public void InitializeItem(Item newItem)
    {
        item = newItem;                                             
        image.sprite = newItem.image;
        InventoryItemPrefab.tag = newItem.tag.ToString();
        
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
        itemTag = InventoryItemPrefab.tag;
        image.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
    }
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;
        transform.SetParent(parentAfterDrag);
    }
}
