using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackObject
{
    public float GetAttackValue(BattleManager.AttackType attackType);
    public float GetMana(bool max = false);
    public float GetStamina(bool max = false);
    public void SetMana(float mana);
    public void SetStamina(float stamina);
    public bool GetIsUsingMana();
    public bool GetIsUsingStamina();
    public void SetIsUsingMana(bool isUsingMana);
    public void SetIsUsingStamina(bool isUsingStamina);
}
