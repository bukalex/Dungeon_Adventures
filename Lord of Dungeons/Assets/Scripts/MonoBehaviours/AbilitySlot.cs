using UnityEngine;
using UnityEngine.EventSystems;

public class AbilitySlot : MonoBehaviour, IDropHandler
{
    public BattleManager.AttackButton attackButton = BattleManager.AttackButton.NONE;
    
    public void OnDrop(PointerEventData eventData)
    {
        if(InventoryManager.Instance.activeAbilities == 0)
        {
            Debug.Log(gameObject);
            GameObject dropped = eventData.pointerDrag;
            AbilityItem abilityItem = dropped?.GetComponent<AbilityItem>();
            abilityItem.parentAfterDrag = transform;
            BattleManager.Instance.AssingAbility(DataManager.Instance.playerData, abilityItem.ability.attackParameters, attackButton);
        }
    }

}
