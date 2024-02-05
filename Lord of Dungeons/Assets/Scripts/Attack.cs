using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack
{
    public BattleManager.AttackType attackType { get; private set; }
    public float duration { get; private set; }
    public float cooldown { get; private set; }
    public float damage { get; private set; }
    public float range { get; private set; }
    public float mana { get; private set; }
    public float stamina { get; private set; }
    public bool isReady { get; set; }

    public Attack(BattleManager.AttackType attackType, float duration, float cooldown, float damage, float range, float mana, float stamina)
    {
        this.attackType = attackType;
        this.duration = duration;
        this.cooldown = cooldown;
        this.damage = damage;
        this.range = range;
        this.mana = mana;
        this.stamina = stamina;

        isReady = true;
    }
}
