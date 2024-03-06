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

    [SerializeField]
    private Image image, imageBackground, clock;
    [SerializeField]
    public Transform parentAfterDrag;
    public Transform parentBeforeDrag;
    public float timer = 1.0f;

    public void InitializeAbility(Ability newAbility)
    {
        ability = newAbility;
        image.sprite = newAbility.backgroundSprite;
        imageBackground.sprite = newAbility.backgroundSprite;
        clock.sprite = newAbility.backgroundSprite;
    }

    void Update()
    {
        if (timer < 1 && ability.attackParameters.duration != 0)
        {
            timer += Time.deltaTime / ability.attackParameters.cooldown;
            clock.fillAmount = timer;
        }
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
