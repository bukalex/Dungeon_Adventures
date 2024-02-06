using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    private Dictionary<PlayerData.CharacterType, Dictionary<AttackButton, Attack>> playerAttacks = new Dictionary<PlayerData.CharacterType, Dictionary<AttackButton, Attack>>();
    private Dictionary<EnemyParameters.EnemyType, Dictionary<AttackButton, Attack>> enemyAttacks = new Dictionary<EnemyParameters.EnemyType, Dictionary<AttackButton, Attack>>();

    private List<Attack> playerRunningAttacks = new List<Attack>();
    private List<Attack> enemyRunningAttacks = new List<Attack>();
    private List<Attack> expiredRunningAttacks = new List<Attack>();

    private Attack attack;
    private ProjectileController projectileController;

    public enum AttackType { BASIC, SPECIAL }
    public enum AttackButton { LMB, RMB, SHIFT }

    void Awake()
    {
        if (Instance == null)
        {
            Initialize();

            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Initialize()
    {
        Dictionary<AttackButton, Attack> playerDict = new Dictionary<AttackButton, Attack>();
        playerDict.Add(AttackButton.LMB, new Attack(AttackType.BASIC, 0.4f, 0.65f, 20.0f, 0.85f, 0.0f, 2.5f));
        playerDict.Add(AttackButton.RMB, new Attack(AttackType.SPECIAL, 3.0f, 6.0f, 0.0f, 0.0f, 2.0f, 0.0f));
        playerDict.Add(AttackButton.SHIFT, new Attack(AttackType.BASIC, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f));
        playerAttacks.Add(PlayerData.CharacterType.WARRIOR, playerDict);

        Dictionary<AttackButton, Attack> guardLMBDict = new Dictionary<AttackButton, Attack>();
        guardLMBDict.Add(AttackButton.LMB, new Attack(AttackType.BASIC, 0.45f, 0.75f, 20.0f, 0.85f, 0.0f, 2.5f));
        guardLMBDict.Add(AttackButton.RMB, new Attack(AttackType.BASIC, 1.0f, 7.5f, 30.0f, 3.0f, 0.0f, 5.0f));
        enemyAttacks.Add(EnemyParameters.EnemyType.GUARD, guardLMBDict);

        Dictionary<AttackButton, Attack> ghostLMBDict = new Dictionary<AttackButton, Attack>();
        ghostLMBDict.Add(AttackButton.LMB, new Attack(AttackType.SPECIAL, 0.0f, 1.5f, 10.0f, 5.0f, 5.0f, 0.0f));
        enemyAttacks.Add(EnemyParameters.EnemyType.GHOST, ghostLMBDict);
    }

    void Update()
    {
        foreach (Attack runningAttack in playerRunningAttacks)
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
                    Debug.Log("2");
                }
                expiredRunningAttacks.Add(runningAttack);
            }
        }

        foreach (Attack runningAttack in enemyRunningAttacks)
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
        foreach (Attack runningAttack in expiredRunningAttacks)
        {
            playerRunningAttacks.Remove(runningAttack);
            enemyRunningAttacks.Remove(runningAttack);
        }
        expiredRunningAttacks.Clear();
    }

    //Performs an attack for LMB
    public bool PlayerPerformLMB(PlayerData playerData)
    {
        attack = playerAttacks[playerData.type][AttackButton.LMB];

        switch (playerData.type)
        {
            case PlayerData.CharacterType.WARRIOR://Simple attack with sword
                if (attack.isReady && AffordAttack(playerData))
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
                else
                {
                    return false;
                }
                break;

            case PlayerData.CharacterType.ARCHER:
                if (attack.isReady && AffordAttack(playerData))
                {

                }
                else
                {
                    return false;
                }
                break;
        }

        if (attack.cooldown > 0)
        {
            StartCoroutine(Cooldown(attack));
        }

        return true;
    }

    //Performs an attack for RMB
    public bool PlayerPerformRMB(PlayerData playerData)
    {
        attack = playerAttacks[playerData.type][AttackButton.RMB];

        switch (playerData.type)
        {
            case PlayerData.CharacterType.WARRIOR://Activate shield
                if (attack.isReady && AffordAttack(playerData))
                {
                    playerData.defense *= 5;
                    playerData.specialDefense *= 5;

                    attack.playerData = playerData;
                    attack.playerEndDelegate = DeactivateShield;

                    StartCoroutine(StartAttack(attack));
                    playerRunningAttacks.Add(attack);
                }
                else
                {
                    return false;
                }
                break;

            case PlayerData.CharacterType.ARCHER:
                if (attack.isReady && AffordAttack(playerData))
                {

                }
                else
                {
                    return false;
                }
                break;
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

    //Performs an enemy`s primary attack
    public bool EnemyPerformLMB(EnemyParameters enemyParameters)
    {
        attack = enemyAttacks[enemyParameters.type][AttackButton.LMB];

        switch (enemyParameters.type)
        {
            case EnemyParameters.EnemyType.GUARD://Simple attack with sword
                if (attack.isReady && AffordAttack(enemyParameters))
                {
                    DealDamage(enemyParameters, enemyParameters.playerData);
                }
                else
                {
                    return false;
                }
                break;

            case EnemyParameters.EnemyType.GHOST://Launches ectoplasm
                if (attack.isReady && AffordAttack(enemyParameters))
                {
                    projectileController = Instantiate(enemyParameters.projectilePrefab, enemyParameters.position, new Quaternion()).GetComponent<ProjectileController>();
                    projectileController.Launch("Enemy", enemyParameters, enemyParameters.playerData, attack, enemyParameters.attackDirection);
                }
                else
                {
                    return false;
                }
                break;
        }

        if (attack.cooldown > 0)
        {
            StartCoroutine(Cooldown(attack));
        }

        return true;
    }

    //Performs an enemy`s secondary attack
    public bool EnemyPerformRMB(EnemyParameters enemyParameters)
    {
        attack = enemyAttacks[enemyParameters.type][AttackButton.LMB];

        switch (enemyParameters.type)
        {
            case EnemyParameters.EnemyType.GUARD://Hits ground stunning player
                if (attack.isReady && AffordAttack(enemyParameters))
                {
                    DealDamage(enemyParameters, enemyParameters.playerData);

                    attack.playerData = enemyParameters.playerData;
                    attack.playerEndDelegate = DisableStun;

                    StartCoroutine(StartAttack(attack));
                    playerRunningAttacks.Add(attack);
                    Debug.Log("1");
                }
                else
                {
                    return false;
                }
                break;

            case EnemyParameters.EnemyType.GHOST:
                if (attack.isReady && AffordAttack(enemyParameters))
                {
                    
                }
                else
                {
                    return false;
                }
                break;
        }

        if (attack.cooldown > 0)
        {
            StartCoroutine(Cooldown(attack));
        }

        return true;
    }

    //Called be projectiles upon hit
    public void ProjectileHit(IAttackObject attackObject, IDefenseObject defenseObject, Attack attack)
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
        
        StartCoroutine(defenseObject.DealDamage(damage, attack.duration));
    }

    //Use this to recharge attacks
    private IEnumerator Cooldown(Attack attack)
    {
        attack.isReady = false;
        yield return new WaitForSeconds(attack.cooldown);
        attack.isReady = true;
    }

    //Use this to start continuous attacks
    private IEnumerator StartAttack(Attack attack)
    {
        attack.isRunning = true;
        yield return new WaitForSeconds(attack.duration);
        attack.isRunning = false;
    }

    private void DeactivateShield(PlayerData playerData)
    {
        playerData.defense /= 5;
        playerData.specialDefense /= 5;
    }

    private void DisableStun(IDefenseObject defenseObject)
    {
        defenseObject.DisableStun();
    }

    public float GetAttackRange(EnemyParameters.EnemyType enemyType, AttackButton attackButton)
    {
        if (enemyAttacks[enemyType].ContainsKey(attackButton))
        {
            return enemyAttacks[enemyType][attackButton].range;
        }

        return Mathf.Infinity;
    }
}
