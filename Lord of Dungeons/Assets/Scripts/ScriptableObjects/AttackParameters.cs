using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Attack Parameters", menuName = "ScriptableObjects/Attack parameters")]
public class AttackParameters : ScriptableObject
{
    public string attackName;
    public PlayerData.CharacterType characterType;
    public EnemyParameters.EnemyType enemyType;
    public BattleManager.AttackButton attackButton = BattleManager.AttackButton.NONE;

    //For continuous attacks
    #region
    public PlayerData playerData { get; set; }
    public PlayerRunningDelegate playerRunningDelegate { get; set; }
    public PlayerEndDelegate playerEndDelegate { get; set; }

    public EnemyParameters enemyParameters { get; set; }
    public EnemyRunningDelegate enemyRunningDelegate { get; set; }
    public EnemyEndDelegate enemyEndDelegate { get; set; }
    #endregion

    [SerializeField]
    private int functionIndex = -1;
    public PlayerAction playerAction;
    public EnemyAction enemyAction;

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
    public bool notEnoughMana = false;
    public bool notEnoughStamina = false;

    public void ResetValues(List<PlayerAction> playerActions, List<EnemyAction> enemyActions)
    {
        if (functionIndex != -1)
        {
            if (playerActions != null)
            {
                playerAction = playerActions[functionIndex];
            }
            else
            {
                enemyAction = enemyActions[functionIndex];
            }
        }
        
        isReady = true;
        isRunning = false;
    }

    public void SetAction(PlayerAction playerAction, EnemyAction enemyAction)
    {
        if (playerAction != null)
        {
            this.playerAction = playerAction;
        }
        else
        {
            this.enemyAction = enemyAction;
        }
    }

    public delegate void PlayerAction(PlayerData playerData, AttackParameters attack);
    public delegate void EnemyAction(EnemyParameters enemyParameters, AttackParameters attack);

    public delegate void PlayerRunningDelegate(PlayerData playerData);
    public delegate void PlayerEndDelegate(PlayerData playerData);
    public delegate void EnemyRunningDelegate(EnemyParameters enemyParameters);
    public delegate void EnemyEndDelegate(EnemyParameters enemyParameters);
}
