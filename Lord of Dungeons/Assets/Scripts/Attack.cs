using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack
{
    //For continuous attacks
    #region
    public PlayerData playerData { get; set; }
    public PlayerRunningDelegate playerRunningDelegate { get; set; }
    public PlayerEndDelegate playerEndDelegate { get; set; }

    public EnemyParameters enemyParameters { get; set; }
    public EnemyRunningDelegate enemyRunningDelegate { get; set; }
    public EnemyEndDelegate enemyEndDelegate { get; set; }
    #endregion

    public BattleManager.AttackType attackType { get; private set; }
    public float timeOffset { get; private set; }
    public float duration { get; private set; }
    public float cooldown { get; private set; }
    public float damage { get; private set; }
    public float range { get; private set; }
    public float mana { get; private set; }
    public float stamina { get; private set; }
    public bool isReady { get; set; }
    public bool isRunning { get; set; }

    public Attack(BattleManager.AttackType attackType, float timeOffset, float duration, float cooldown, float damage, float range, float mana, float stamina)
    {
        this.attackType = attackType;
        this.timeOffset = timeOffset;
        this.duration = duration;
        this.cooldown = cooldown;
        this.damage = damage;
        this.range = range;
        this.mana = mana;
        this.stamina = stamina;

        isReady = true;
        isRunning = false;
    }

    public delegate void PlayerRunningDelegate(PlayerData playerData);
    public delegate void PlayerEndDelegate(PlayerData playerData);
    public delegate void EnemyRunningDelegate(EnemyParameters enemyParameters);
    public delegate void EnemyEndDelegate(EnemyParameters enemyParameters);
}
