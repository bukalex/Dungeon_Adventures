using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts.Enums;

namespace Assets.Scripts.InventoryElements
{
    public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        public TMP_Text countText;
        [SerializeField]
        public GameObject InventoryItemPrefab;
        [SerializeField]
        private int Id;
        [SerializeField]
        public Dictionary<int, List<ModifierID>> itemModifiers;

        [HideInInspector]
        public Item item;
        [HideInInspector]
        public Image image;
        [HideInInspector]
        public Transform parentAfterDrag;
        [HideInInspector]
        public string itemTag;
        [HideInInspector]
        public bool isLocked = false;

        public void InitializeItem(Item newItem)
        {
            Item item = newItem;
            image.sprite = newItem.sprite;
            Id = item.Id;
            itemModifiers.Add(newItem.Id, newItem.Modifier);

            updateCount(newItem);
        }

        public int GetItemID(Item item)
        {
            return item.Id;
        }

        public List<ModifierID> GetItemModifiers(Item item)
        {
            return itemModifiers[item.Id];
        }

        public void updateCount(Item item)
        {
            countText.text = item.Count.ToString();
            bool textActive = item.Count > 1;
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
}
