using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    public Item item;
    [SerializeField]
    public TMP_Text countText;
    [SerializeField]
    public GameObject InventoryItemPrefab;
    [SerializeField]
    public int materialID;

    public GameObject ItemDescription;

    [HideInInspector]
    public Image image;
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
        materialID = newItem.materialID;
        
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
    private IEnumerator onItemDescription(float interval)
    {
        yield return new WaitForSeconds(interval);
        ItemDescription.SetActive(true);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        onItemDescription(0.75f);
        ItemDescription.SetActive(true);
        ItemDescription.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ItemDescription.SetActive(false);
    }
}
