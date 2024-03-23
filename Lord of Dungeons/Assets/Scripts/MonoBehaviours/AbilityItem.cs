using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class AbilityItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    public Ability ability;

    [SerializeField]
    private Image image, imageBackground, clock;
    [SerializeField]
    public Transform parentAfterDrag;
    public Transform parentBeforeDrag;
    public float timer = 1.0f;

    public void InitializeAbility(Ability newAbility, int rank = 1)
    {
        ability = Instantiate(newAbility);
        ability.attackParameters = Instantiate(newAbility.attackParameters);
        ability.attackParameters.SetRank(rank);

        image.sprite = newAbility.maskSprite;
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
            BattleManager.Instance.isUsingUI = true;
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
            BattleManager.Instance.isUsingUI = false;
            transform?.SetParent(parentAfterDrag);
        }
    }

    private IEnumerator ItemDescriptionOn(float interval)
    {
        yield return new WaitForSeconds(interval);
        InventoryManager.Instance.ItemDescription.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(ItemDescriptionOn(0.75f));
        InventoryManager.Instance.InitializeAbilityDescription(ability);
        Debug.Log("Enter");

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        InventoryManager.Instance.ItemDescription.SetActive(false);
    }
}
