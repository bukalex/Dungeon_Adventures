using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Parameters", menuName = "ScriptableObjects/Attack parameters")]
public class AttackParameters : ScriptableObject
{
    public PlayerData.CharacterType characterType;
    public EnemyParameters.EnemyType enemyType;
    public BattleManager.AttackButton attackButton;

    //For continuous attacks
    #region
    public PlayerData playerData { get; set; }
    public PlayerRunningDelegate playerRunningDelegate { get; set; }
    public PlayerEndDelegate playerEndDelegate { get; set; }

    public EnemyParameters enemyParameters { get; set; }
    public EnemyRunningDelegate enemyRunningDelegate { get; set; }
    public EnemyEndDelegate enemyEndDelegate { get; set; }
    #endregion

    public BattleManager.AttackType attackType;
    public float timeOffset;
    public float duration;
    public float cooldown;
    public float damage;
    public float range;
    public float mana;
    public float stamina;
    public bool isReady = true;
    public bool isRunning = false;

    public void ResetValues()
    {
        isReady = true;
        isRunning = false;
    }

    public delegate void PlayerRunningDelegate(PlayerData playerData);
    public delegate void PlayerEndDelegate(PlayerData playerData);
    public delegate void EnemyRunningDelegate(EnemyParameters enemyParameters);
    public delegate void EnemyEndDelegate(EnemyParameters enemyParameters);
}
