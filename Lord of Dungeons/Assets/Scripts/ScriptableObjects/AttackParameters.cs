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
    public EnemyParameters enemyParameters { get; set; }
    public List<EnemyParameters> enemyParametersList { get; set; }

    public RunningDelegate runningDelegate { get; set; }
    public EndDelegate endDelegate { get; set; }
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

    public void SetAction(List<PlayerAction> playerActions, List<EnemyAction> enemyActions)
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
    }

    public delegate void PlayerAction(PlayerData playerData, AttackParameters attack);
    public delegate void EnemyAction(EnemyParameters enemyParameters, AttackParameters attack);

    public delegate void RunningDelegate(AttackParameters attack);
    public delegate void EndDelegate(AttackParameters attack);
}
