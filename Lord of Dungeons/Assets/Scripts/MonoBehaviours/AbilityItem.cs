using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class AbilityItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    public Ability ability;

    [HideInInspector]
    public Image image;
    [SerializeField]
    public Transform parentAfterDrag;
    public Transform parentBeforeDrag;

    public void InitializeAbility(Ability newAbility)
    {
        ability = newAbility;
        image.sprite = newAbility.backgroundSprite;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (InventoryManager.Instance.activeAbilities == 0)
        {
            image.raycastTarget = false;
            parentBeforeDrag = transform.parent;
            parentAfterDrag = transform.parent;
            transform.SetParent(transform.root);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (InventoryManager.Instance.activeAbilities == 0)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (InventoryManager.Instance.activeAbilities == 0)
        {
            AbilityItem existingAbility = parentAfterDrag.GetComponentInChildren<AbilityItem>();
            if (existingAbility != null) existingAbility.transform.SetParent(parentBeforeDrag);

            image.raycastTarget = true;
            transform.SetParent(parentAfterDrag);
        }
    }
}
