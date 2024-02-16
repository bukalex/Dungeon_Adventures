using UnityEngine;
using UnityEngine.EventSystems;

public class AbilitySlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if(transform.childCount == 0)
        {
            GameObject dropped = eventData.pointerDrag;
            AbilityItem abilityItem = dropped.GetComponent<AbilityItem>();
            abilityItem.parentAfterDrag = transform;
        }
    }

}
