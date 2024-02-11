using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [SerializeField]
    private List<AttackParameters> playerAttackParameters = new List<AttackParameters>();
    [SerializeField]
    private List<AttackParameters> enemyAttackParameters = new List<AttackParameters>();

    private Dictionary<PlayerData.CharacterType, List<AttackParameters>> playerAttacksAll = new Dictionary<PlayerData.CharacterType, List<AttackParameters>>();
    public List<AttackParameters.PlayerAction> playerActions = new List<AttackParameters.PlayerAction>();
    public List<AttackParameters.EnemyAction> enemyActions = new List<AttackParameters.EnemyAction>();

    private Dictionary<PlayerData.CharacterType, Dictionary<AttackButton, AttackParameters>> playerAttacks = new Dictionary<PlayerData.CharacterType, Dictionary<AttackButton, AttackParameters>>();
    private Dictionary<EnemyParameters.EnemyType, Dictionary<AttackButton, AttackParameters>> enemyAttacks = new Dictionary<EnemyParameters.EnemyType, Dictionary<AttackButton, AttackParameters>>();

    private List<AttackParameters> playerRunningAttacks = new List<AttackParameters>();
    private List<AttackParameters> enemyRunningAttacks = new List<AttackParameters>();
    private List<AttackParameters> expiredRunningAttacks = new List<AttackParameters>();

    private AttackParameters attack;
    private ProjectileController projectileController;

    public enum AttackType { BASIC, SPECIAL }
    public enum AttackButton { NONE, LMB, RMB, SHIFT }

    void Awake()
    {
        if (Instance == null)
        {
            Initialize();
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        playerActions.Add(PlayerUseSword);
        playerActions.Add(PlayerActivateShield);

        enemyActions.Add(GuardUseSword);
        enemyActions.Add(GuardUseSpecial);
        enemyActions.Add(GhostShoot);

        foreach (string str in Enum.GetNames(typeof(PlayerData.CharacterType)))
        {
            List<AttackParameters> classAttacks = new List<AttackParameters>();
            Dictionary<AttackButton, AttackParameters> playerDict = new Dictionary<AttackButton, AttackParameters>();

            foreach (AttackParameters attackParameters in playerAttackParameters)
            {
                if (attackParameters.characterType == (PlayerData.CharacterType)Enum.Parse(typeof(PlayerData.CharacterType), str))
                {
                    attackParameters.ResetValues(playerActions, null);
                    classAttacks.Add(attackParameters);

                    if (attackParameters.attackButton != AttackButton.NONE)
                    {
                        playerDict.Add(attackParameters.attackButton, attackParameters);
                    }
                }
            }

            playerAttacksAll.Add((PlayerData.CharacterType)Enum.Parse(typeof(PlayerData.CharacterType), str), classAttacks);
            playerAttacks.Add((PlayerData.CharacterType)Enum.Parse(typeof(PlayerData.CharacterType), str), playerDict);
        }

        foreach (string str in Enum.GetNames(typeof(EnemyParameters.EnemyType)))
        {
            Dictionary<AttackButton, AttackParameters> enemyDict = new Dictionary<AttackButton, AttackParameters>();

            foreach (AttackParameters attackParameters in enemyAttackParameters)
            {
                if (attackParameters.enemyType == (EnemyParameters.EnemyType)Enum.Parse(typeof(EnemyParameters.EnemyType), str))
                {
                    attackParameters.ResetValues(null, enemyActions);
                    enemyDict.Add(attackParameters.attackButton, attackParameters);
                }
            }

            enemyAttacks.Add((EnemyParameters.EnemyType)Enum.Parse(typeof(EnemyParameters.EnemyType), str), enemyDict);
        }
    }

    void Update()
    {
        foreach (AttackParameters runningAttack in playerRunningAttacks)
        {
            //Calls functions for attacks that have continuous effect
            if (runningAttack.isRunning)
            {
                if (runningAttack.playerRunningDelegate != null)
                {
                    runningAttack.playerRunningDelegate(runningAttack.playerData);
                }
            }
            //Calls functions for attacks that have effect upon end of their duration
            else
            {
                if (runningAttack.playerEndDelegate != null)
                {
                    runningAttack.playerEndDelegate(runningAttack.playerData);
                }
                expiredRunningAttacks.Add(runningAttack);
            }
        }

        foreach (AttackParameters runningAttack in enemyRunningAttacks)
        {
            //Calls functions for attacks that have continuous effect
            if (runningAttack.isRunning)
            {
                if (runningAttack.playerRunningDelegate != null)
                {
                    runningAttack.enemyRunningDelegate(runningAttack.enemyParameters);
                }
            }
            //Calls functions for attacks that have effect upon end of their duration
            else
            {
                if (runningAttack.playerEndDelegate != null)
                {
                    runningAttack.enemyEndDelegate(runningAttack.enemyParameters);
                }
                expiredRunningAttacks.Add(runningAttack);
            }
        }

        //Removes expired attack
        foreach (AttackParameters runningAttack in expiredRunningAttacks)
        {
            playerRunningAttacks.Remove(runningAttack);
            enemyRunningAttacks.Remove(runningAttack);
        }
        expiredRunningAttacks.Clear();
    }

    public bool PlayerPerformAction(PlayerData playerData, AttackButton attackButton)
    {
        if (playerAttacks.ContainsKey(playerData.type) && playerAttacks[playerData.type].ContainsKey(attackButton))
        {
            attack = playerAttacks[playerData.type][attackButton];
        }
        else
        {
            return false;
        }

        if (attack.isReady && AffordAttack(playerData))
        {
            attack.playerAction(playerData);
        }
        else
        {
            return false;
        }

        if (attack.cooldown > 0)
        {
            StartCoroutine(Cooldown(attack));
        }

        return true;
    }

    //Performs an attack for Shift
    public bool PlayerPerformShift(PlayerData playerData)
    {
        //Run
        attack = playerAttacks[playerData.type][AttackButton.SHIFT];
        return AffordAttack(playerData, true);
    }

    public bool EnemyPerformAction(EnemyParameters enemyParameters, AttackButton attackButton)
    {
        if (enemyAttacks.ContainsKey(enemyParameters.type) && enemyAttacks[enemyParameters.type].ContainsKey(attackButton))
        {
            attack = enemyAttacks[enemyParameters.type][attackButton];
        }
        else
        {
            return false;
        }

        if (attack.isReady && AffordAttack(enemyParameters))
        {
            attack.enemyAction(enemyParameters);
        }
        else
        {
            return false;
        }

        if (attack.cooldown > 0)
        {
            StartCoroutine(Cooldown(attack));
        }

        return true;
    }

    //Called be projectiles upon hit
    public void ProjectileHit(IAttackObject attackObject, IDefenseObject defenseObject, AttackParameters attack)
    {
        this.attack = attack;
        DealDamage(attackObject, defenseObject);
    }

    //Finds specified targets
    private List<T> DetectTargets<T>(PlayerData playerData, bool inSector = true)
    {
        Collider2D[] possibleTargets = Physics2D.OverlapCircleAll(playerData.position, attack.range + playerData.colliderRadius);
        List<T> targets = new List<T>();

        foreach (Collider2D target in possibleTargets)
        {
            if (target.GetComponent<T>() != null)
            {
                Vector2 targetDirection = target.transform.position - playerData.position;

                if (!inSector)
                {
                    targets.Add(target.GetComponent<T>());
                }
                else if (Vector2.Angle(playerData.attackDirection, targetDirection) <= 45)
                {
                    targets.Add(target.GetComponent<T>());
                }
            }
        }

        return targets;
    }

    //Checks if attack can be performed and takes the required amount of mana or stamina if returns true
    private bool AffordAttack(IAttackObject attackObject, bool isPerFrame = false)
    {
        float multiplier = 1.0f;
        if (isPerFrame)
        {
            multiplier = Time.deltaTime;
        }

        if (attack.mana != 0)
        {
            attackObject.SetIsUsingMana(attack.mana * multiplier <= attackObject.GetMana());
            if (!attackObject.GetIsUsingMana())
            {
                return false;
            }
        }

        if (attack.stamina != 0)
        {
            attackObject.SetIsUsingStamina(attack.stamina * multiplier <= attackObject.GetStamina());
            if (!attackObject.GetIsUsingStamina())
            {
                return false;
            }
        }

        if (attackObject.GetIsUsingMana())
        {
            attackObject.SetMana(Mathf.Clamp(attackObject.GetMana() - attack.mana * multiplier, 0, attackObject.GetMana(true)));
        }
        if (attackObject.GetIsUsingStamina())
        {
            attackObject.SetStamina(Mathf.Clamp(attackObject.GetStamina() - attack.stamina * multiplier, 0, attackObject.GetStamina(true)));
        }

        return true;
    }

    //Deals damage from attackObject to defenseObject
    private void DealDamage(IAttackObject attackObject, IDefenseObject defenseObject)
    {
        float damage = attack.damage;
        damage *= 1.0f + (attackObject.GetAttackValue(attack.attackType) - defenseObject.GetDefenseValue(attack.attackType)) * 0.05f;

        if (damage < 1.0f)
        {
            damage = 1.0f;
        }
        
        StartCoroutine(defenseObject.DealDamage(damage, attack.timeOffset));
    }

    //Use this to recharge attacks
    private IEnumerator Cooldown(AttackParameters attack)
    {
        attack.isReady = false;
        yield return new WaitForSeconds(attack.cooldown);
        attack.isReady = true;
    }

    //Use this to start continuous attacks
    private IEnumerator StartAttack(AttackParameters attack)
    {
        attack.isRunning = true;
        yield return new WaitForSeconds(attack.duration);
        attack.isRunning = false;
    }

    public float GetAttackRange(EnemyParameters.EnemyType enemyType, AttackButton attackButton)
    {
        if (enemyAttacks[enemyType].ContainsKey(attackButton))
        {
            return enemyAttacks[enemyType][attackButton].range;
        }

        return Mathf.Infinity;
    }

    public void PlayerUseSword(PlayerData playerData)
    {
        List<EnemyController> enemies = DetectTargets<EnemyController>(playerData);
        foreach (EnemyController enemy in enemies)
        {
            if (enemy.IsAlive())
            {
                DealDamage(playerData, enemy.enemyParameters);
            }
        }

        List<ObjectController> breakables = DetectTargets<ObjectController>(playerData);
        foreach (ObjectController breakable in breakables)
        {
            if (breakable.IsIntact())
            {
                DealDamage(playerData, breakable.objectParameters);
            }
        }
    }

    public void PlayerActivateShield(PlayerData playerData)
    {
        playerData.defense *= 5;
        playerData.specialDefense *= 5;

        attack.playerData = playerData;
        attack.playerEndDelegate = PlayerDeactivateShield;

        StartCoroutine(StartAttack(attack));
        playerRunningAttacks.Add(attack);
    }

    public void GuardUseSword(EnemyParameters enemyParameters)
    {
        DealDamage(enemyParameters, enemyParameters.playerData);
    }

    public void GuardUseSpecial(EnemyParameters enemyParameters)
    {
        enemyParameters.playerData.isStunned = true;
        DealDamage(enemyParameters, enemyParameters.playerData);

        attack.playerData = enemyParameters.playerData;
        attack.playerEndDelegate = DisableStun;

        StartCoroutine(StartAttack(attack));
        playerRunningAttacks.Add(attack);
    }

    public void GhostShoot(EnemyParameters enemyParameters)
    {
        projectileController = Instantiate(enemyParameters.projectilePrefab, enemyParameters.position, new Quaternion()).GetComponent<ProjectileController>();
        projectileController.Launch("Enemy", enemyParameters, enemyParameters.playerData, attack, enemyParameters.attackDirection);
    }

    private void PlayerDeactivateShield(PlayerData playerData)
    {
        playerData.defense /= 5;
        playerData.specialDefense /= 5;
    }

    private void DisableStun(IDefenseObject defenseObject)
    {
        defenseObject.DisableStun();
    }
}
