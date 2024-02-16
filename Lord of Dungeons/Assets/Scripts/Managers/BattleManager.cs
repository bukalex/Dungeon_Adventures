using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BattleManager;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [SerializeField]
    private BattleData battleData;
    [SerializeField]
    private List<AttackParameters> playerAttackParameters = new List<AttackParameters>();
    [SerializeField]
    private List<AttackParameters> enemyAttackParameters = new List<AttackParameters>();

    private Dictionary<PlayerData.CharacterType, List<AttackParameters>> playerAttacksAll = new Dictionary<PlayerData.CharacterType, List<AttackParameters>>();
    private List<AttackParameters.PlayerAction> playerActions = new List<AttackParameters.PlayerAction>();
    private List<AttackParameters.EnemyAction> enemyActions = new List<AttackParameters.EnemyAction>();

    private Dictionary<PlayerData.CharacterType, Dictionary<AttackButton, AttackParameters>> playerAttacks = new Dictionary<PlayerData.CharacterType, Dictionary<AttackButton, AttackParameters>>();
    private Dictionary<EnemyParameters.EnemyType, Dictionary<AttackButton, AttackParameters>> enemyAttacks = new Dictionary<EnemyParameters.EnemyType, Dictionary<AttackButton, AttackParameters>>();

    private List<AttackParameters> playerRunningAttacks = new List<AttackParameters>();
    private List<AttackParameters> enemyRunningAttacks = new List<AttackParameters>();
    private List<AttackParameters> expiredRunningAttacks = new List<AttackParameters>();

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

    private Dictionary<T, Dictionary<AttackButton, AttackParameters>> CloneDictionary<T>(Dictionary<T, Dictionary<AttackButton, AttackParameters>> originalDictionary)
    {
        Dictionary<T, Dictionary<AttackButton, AttackParameters>> copy = new Dictionary<T, Dictionary<AttackButton, AttackParameters>>();
        
        foreach (KeyValuePair<T, Dictionary<AttackButton, AttackParameters>> outPair in originalDictionary)
        {
            Dictionary<AttackButton, AttackParameters> inCopy = new Dictionary<AttackButton, AttackParameters>();
            foreach (KeyValuePair<AttackButton, AttackParameters> inPair in outPair.Value)
            {
                inCopy.Add(inPair.Key, Instantiate(inPair.Value));
            }
            copy.Add(outPair.Key, inCopy);
        }
        
        return copy;
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
                runningAttack.playerRunningDelegate = null;
                runningAttack.playerEndDelegate = null;
                expiredRunningAttacks.Add(runningAttack);
            }
        }

        foreach (AttackParameters runningAttack in enemyRunningAttacks)
        {
            //Calls functions for attacks that have continuous effect
            if (runningAttack.isRunning)
            {
                if (runningAttack.enemyRunningDelegate != null)
                {
                    runningAttack.enemyRunningDelegate(runningAttack.enemyParameters);
                }
            }
            //Calls functions for attacks that have effect upon end of their duration
            else
            {
                if (runningAttack.enemyEndDelegate != null)
                {
                    runningAttack.enemyEndDelegate(runningAttack.enemyParameters);
                }
                runningAttack.enemyRunningDelegate = null;
                runningAttack.enemyEndDelegate = null;
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
        AttackParameters attack;

        if (playerData.attacks == null)
        {
            playerData.attacks = CloneDictionary(playerAttacks);
        }

        if (playerAttacks.ContainsKey(playerData.type) && playerAttacks[playerData.type].ContainsKey(attackButton))
        {
            attack = playerData.attacks[playerData.type][attackButton];
            attack.SetAction(playerAttacks[playerData.type][attackButton].playerAction, null);
        }
        else
        {
            return false;
        }

        if (attack.isReady && AffordAttack(playerData, attack))
        {
            StartCoroutine(DelayAttack(attack, playerData, null));
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
        AttackParameters attack;

        if (playerData.attacks == null)
        {
            playerData.attacks = CloneDictionary(playerAttacks);
        }

        //Run
        if (playerAttacks.ContainsKey(playerData.type))
        {
            attack = playerData.attacks[playerData.type][AttackButton.SHIFT];
            return AffordAttack(playerData, attack, true);
        }
        else
        {
            return false;
        }
    }

    public bool EnemyPerformAction(EnemyParameters enemyParameters, AttackButton attackButton)
    {
        AttackParameters attack;

        if (enemyParameters.attacks == null)
        {
            enemyParameters.attacks = CloneDictionary(enemyAttacks);
        }

        if (enemyAttacks.ContainsKey(enemyParameters.type) && enemyAttacks[enemyParameters.type].ContainsKey(attackButton))
        {
            attack = enemyParameters.attacks[enemyParameters.type][attackButton];
            attack.SetAction(null, enemyAttacks[enemyParameters.type][attackButton].enemyAction);
        }
        else
        {
            return false;
        }

        if (attack.isReady && AffordAttack(enemyParameters, attack))
        {
            StartCoroutine(DelayAttack(attack, null, enemyParameters));
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
        DealDamage(attackObject, defenseObject, attack);
    }

    //Finds specified targets
    private List<T> DetectTargets<T>(Vector3 position, float radius, Vector3 attackDirection, bool inSector = true)
    {
        Collider2D[] possibleTargets = Physics2D.OverlapCircleAll(position, radius);
        List<T> targets = new List<T>();

        foreach (Collider2D target in possibleTargets)
        {
            if (target.isTrigger && target.GetComponentInParent<T>() != null)
            {
                Vector2 targetDirection = target.transform.parent.position - position;
                if (!inSector)
                {
                    targets.Add(target.GetComponentInParent<T>());
                }
                else if (Vector2.Angle(attackDirection, targetDirection) <= 45)
                {
                    targets.Add(target.GetComponentInParent<T>());
                }
            }
        }

        return targets;
    }

    //Checks if attack can be performed and takes the required amount of mana or stamina if returns true
    private bool AffordAttack(IAttackObject attackObject, AttackParameters attack, bool isPerFrame = false)
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
            if (attack.notEnoughStamina)
            {
                if (attack.stamina > attackObject.GetStamina())
                {
                    return false;
                }
                else
                {
                    attack.notEnoughStamina = false;
                }
            }

            attackObject.SetIsUsingStamina(attack.stamina * multiplier <= attackObject.GetStamina());
            if (!attackObject.GetIsUsingStamina())
            {
                attack.notEnoughStamina = true;
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
    private void DealDamage(IAttackObject attackObject, IDefenseObject defenseObject, AttackParameters attack)
    {
        float damage = attack.damage;
        damage *= 1.0f + (attackObject.GetAttackValue(attack.attackType) - defenseObject.GetDefenseValue(attack.attackType)) * 0.05f;

        if (damage < 1.0f)
        {
            damage = 1.0f;
        }
        
        defenseObject.DealDamage(damage);
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

    //Use this for attack`s time offset
    private IEnumerator DelayAttack(AttackParameters attack, PlayerData playerData, EnemyParameters enemyParameters)
    {
        yield return new WaitForSeconds(attack.timeOffset);

        if (enemyParameters == null)
        {
            attack.playerAction(playerData, attack);
        }
        else
        {
            attack.enemyAction(enemyParameters, attack);
        }
    }

    public float GetAttackRange(EnemyParameters.EnemyType enemyType, AttackButton attackButton)
    {
        if (enemyAttacks[enemyType].ContainsKey(attackButton))
        {
            return enemyAttacks[enemyType][attackButton].range;
        }

        return Mathf.Infinity;
    }

    private void PlayerUseSword(PlayerData playerData, AttackParameters attack)
    {
        bool shakeCamera = false;
        bool stopAnimation = false;

        List<EnemyController> enemies = DetectTargets<EnemyController>(playerData.position, attack.range + playerData.colliderRadius, playerData.attackDirection);
        foreach (EnemyController enemy in enemies)
        {
            if (enemy.IsAlive())
            {
                shakeCamera = true;
                DealDamage(playerData, enemy.enemyParameters, attack);
            }
        }

        List<ObjectController> breakables = DetectTargets<ObjectController>(playerData.position, attack.range + playerData.colliderRadius, playerData.attackDirection);
        foreach (ObjectController breakable in breakables)
        {
            if (breakable.IsIntact())
            {
                shakeCamera = true;
                DealDamage(playerData, breakable.objectParameters, attack);
            }
        }

        if (shakeCamera)
        {
            Camera.main.GetComponent<Animator>().SetTrigger("shake");
        }

        foreach (EnemyController enemy in enemies)
        {
            if (enemy.IsAlive())
            {
                stopAnimation = true;
            }
        }

        foreach (ObjectController breakable in breakables)
        {
            if (breakable.IsIntact())
            {
                stopAnimation = true;
            }
        }

        if (stopAnimation)
        {
            playerData.transform.GetComponentInChildren<Animator>().Play("Idle");
        }
    }

    private void PlayerActivateShield(PlayerData playerData, AttackParameters attack)
    {
        GameObject shield = Instantiate(battleData.shieldPrefab, playerData.transform);
        shield.transform.localPosition = new Vector3(0, 0.5f, 0);
        shield.GetComponent<Animator>().SetBool("shieldActivated", true);
        battleData.shieldsByCreatures.Add(playerData, shield);

        playerData.defense *= 5;
        playerData.specialDefense *= 5;

        attack.playerData = playerData;
        attack.playerEndDelegate = PlayerDeactivateShield;

        StartCoroutine(StartAttack(attack));
        playerRunningAttacks.Add(attack);
    }

    private void PlayerDeactivateShield(PlayerData playerData)
    {
        GameObject shield = battleData.shieldsByCreatures[playerData];
        shield.GetComponent<Animator>().SetBool("shieldActivated", false);
        Destroy(shield, 0.5f);
        battleData.shieldsByCreatures.Remove(playerData);

        playerData.defense /= 5;
        playerData.specialDefense /= 5;
    }

    private void GuardUseSword(EnemyParameters enemyParameters, AttackParameters attack)
    {
        List<PlayerController> players = DetectTargets<PlayerController>(enemyParameters.position, attack.range + enemyParameters.colliderRadius + 0.25f, enemyParameters.attackDirection);
        foreach (PlayerController player in players)
        {
            if (player.GetPlayerData().IsAlive())
            {
                DealDamage(enemyParameters, player.GetPlayerData(), attack);
            }
        }
    }

    private void GuardUseSpecial(EnemyParameters enemyParameters, AttackParameters attack)
    {
        GameObject area = Instantiate(battleData.superAttackAreaPrefab, enemyParameters.transform);
        area.transform.localPosition = new Vector3(0, 0, 0);
        Destroy(area, 0.75f);

        enemyParameters.playerData.isStunned = true;
        DealDamage(enemyParameters, enemyParameters.playerData, attack);

        attack.playerData = enemyParameters.playerData;
        attack.playerEndDelegate = DisableStun;

        StartCoroutine(StartAttack(attack));
        playerRunningAttacks.Add(attack);
    }

    private void GhostShoot(EnemyParameters enemyParameters, AttackParameters attack)
    {
        projectileController = Instantiate(enemyParameters.projectilePrefab, enemyParameters.position, new Quaternion()).GetComponent<ProjectileController>();
        projectileController.Launch("Enemy", enemyParameters, enemyParameters.playerData, attack, enemyParameters.attackDirection);
    }

    private void DisableStun(IDefenseObject defenseObject)
    {
        defenseObject.DisableStun();
    }
}
