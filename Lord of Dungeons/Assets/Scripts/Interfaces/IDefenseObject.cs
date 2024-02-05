using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDefenseObject
{
    public float GetDefenseValue(BattleManager.AttackType attackType);
    public void DealDamage(float damage);
}
