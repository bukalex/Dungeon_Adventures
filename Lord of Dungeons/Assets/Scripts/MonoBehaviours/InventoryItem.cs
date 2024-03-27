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


    public Image image;
    public int count = 1;
    [HideInInspector]
    public Transform parentAfterDrag;
    private Transform parentBeforeDrag;
    [HideInInspector]
    public string itemTag;
    [HideInInspector]
    public bool isLocked = false;
    private void Update()
    {
        
    }
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
        if (eventData.button == PointerEventData.InputButton.Right)
        {

        }

        if (!isLocked)
        {
            itemTag = InventoryItemPrefab.tag;
            image.raycastTarget = false;
            BattleManager.Instance.isUsingUI = true;
            parentAfterDrag = transform.parent;
            parentBeforeDrag = transform.parent;
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
            BattleManager.Instance.isUsingUI = false;
            transform.SetParent(parentAfterDrag);
            if (TrainingManager.Instance != null && parentAfterDrag != parentBeforeDrag) TrainingManager.Instance.itemWasDraggedAndMoved = true;
        }
    }
    private IEnumerator ItemDescriptionOn(float interval)
    {
        yield return new WaitForSeconds(interval);
        InventoryManager.Instance.ItemDescription.SetActive(true);
    }
    private IEnumerator ItemDescriptionOff(float interval)
    {
        yield return new WaitForSeconds(interval);
        InventoryManager.Instance.ItemDescription.SetActive(false);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        InventoryManager.Instance.currentInventoryItem = transform.gameObject.GetComponent<InventoryItem>();
        InventoryManager.Instance.itemToChange = item;
        StartCoroutine(ItemDescriptionOn(0.75f));
        InventoryManager.Instance.InitializeItemDescription(item);

        InventoryManager.Instance.itemToChange = item;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        InventoryManager.Instance.ItemDescription.SetActive(false);

        InventoryManager.Instance.itemToChange = null;
    }
}
