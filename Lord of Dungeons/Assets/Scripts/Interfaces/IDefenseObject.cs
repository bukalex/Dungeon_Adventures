using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDefenseObject
{
    public float GetDefenseValue(BattleManager.AttackType attackType);
    public float DealDamage(float damage);
    public void DisableStun();
    public Vector3 GetPosition();
}
