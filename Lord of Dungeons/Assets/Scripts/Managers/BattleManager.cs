using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [SerializeField]
    private BattleData battleData;
    [SerializeField]
    private List<AttackParameters> playerAttackParameters = new List<AttackParameters>();
    [SerializeField]
    private List<AttackParameters> enemyAttackParameters = new List<AttackParameters>();

    private List<AttackParameters.PlayerAction> playerActions = new List<AttackParameters.PlayerAction>();
    private List<AttackParameters.EnemyAction> enemyActions = new List<AttackParameters.EnemyAction>();

    private Dictionary<PlayerData.CharacterType, Dictionary<AttackButton, AttackParameters>> playerAttacks = new Dictionary<PlayerData.CharacterType, Dictionary<AttackButton, AttackParameters>>();
    private Dictionary<EnemyParameters.EnemyType, Dictionary<AttackButton, AttackParameters>> enemyAttacks = new Dictionary<EnemyParameters.EnemyType, Dictionary<AttackButton, AttackParameters>>();

    private List<AttackParameters> runningAttacks = new List<AttackParameters>();
    private List<AttackParameters> expiredAttacks = new List<AttackParameters>();

    public enum AttackType { BASIC, SPECIAL }
    public enum AttackButton { NONE, LMB, RMB, SHIFT, R, F, C, V }

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
        playerActions.Add(PlayerLeap);
        playerActions.Add(PlayerBoomerang);
        playerActions.Add(PlayerDash);
        playerActions.Add(PlayerExplosionClosest);
        playerActions.Add(PlayerPushingWave);
        playerActions.Add(PlayerGuisonKnife);
        playerActions.Add(PlayerMindControl);
        playerActions.Add(PlayerBlock);

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
                    classAttacks.Add(attackParameters);

                    if (attackParameters.attackButton != AttackButton.NONE)
                    {
                        playerDict.Add(attackParameters.attackButton, attackParameters);
                    }
                }
            }

            playerAttacks.Add((PlayerData.CharacterType)Enum.Parse(typeof(PlayerData.CharacterType), str), playerDict);
        }

        foreach (string str in Enum.GetNames(typeof(EnemyParameters.EnemyType)))
        {
            Dictionary<AttackButton, AttackParameters> enemyDict = new Dictionary<AttackButton, AttackParameters>();

            foreach (AttackParameters attackParameters in enemyAttackParameters)
            {
                if (attackParameters.enemyType == (EnemyParameters.EnemyType)Enum.Parse(typeof(EnemyParameters.EnemyType), str))
                {
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

    void LateUpdate()
    {
        foreach (AttackParameters attack in runningAttacks)
        {
            //Calls functions for attacks that have continuous effect
            if (attack.isRunning)
            {
                if (attack.runningDelegate != null)
                {
                    attack.runningDelegate(attack);
                }
            }
            //Calls functions for attacks that have effect upon end of their duration
            else
            {
                if (attack.endDelegate != null)
                {
                    attack.endDelegate(attack);
                }
                attack.runningDelegate = null;
                attack.endDelegate = null;
                expiredAttacks.Add(attack);
            }
        }

        //Removes expired attack
        foreach (AttackParameters attack in expiredAttacks)
        {
            runningAttacks.Remove(attack);
        }
        expiredAttacks.Clear();
    }

    public bool PlayerPerformAction(PlayerData playerData, AttackButton attackButton)
    {
        AttackParameters attack;

        if (playerData.attacks == null)
        {
            playerData.attacks = CloneDictionary(playerAttacks);
        }
        
        if (playerData.attacks.ContainsKey(playerData.type) && playerData.attacks[playerData.type].ContainsKey(attackButton))
        {
            attack = playerData.attacks[playerData.type][attackButton];
            attack.SetAction(playerActions, null);
        }
        else
        {
            return false;
        }
        
        if (attack.isReady && AffordAttack(playerData, attack, attack.isPerSecond))
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
        if (playerData.attacks.ContainsKey(playerData.type))
        {
            attack = playerData.attacks[playerData.type][AttackButton.SHIFT];
            return AffordAttack(playerData, attack, attack.isPerSecond);
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

        if (enemyParameters.attacks.ContainsKey(enemyParameters.type) && enemyParameters.attacks[enemyParameters.type].ContainsKey(attackButton))
        {
            attack = enemyParameters.attacks[enemyParameters.type][attackButton];
            attack.SetAction(null, enemyActions);
        }
        else
        {
            return false;
        }

        if (attack.isReady && AffordAttack(enemyParameters, attack))
        {
            enemyParameters.isAttacking = true;
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
    public void ProjectileHit(IAttackObject attackObject, IDefenseObject defenseObject, AttackParameters attack, bool isPerFrame = false)
    {
        DealDamage(attackObject, defenseObject, attack, isPerFrame);
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
                if (!inSector || Vector2.Angle(attackDirection, targetDirection) <= 45)
                {
                    targets.Add(target.GetComponentInParent<T>());
                }
            }
        }

        return targets;
    }

    //Finds nearest target
    private T GetNearestTarget<T>(Vector3 position, float radius, Vector3 attackDirection, bool inSector = true, bool mustBeVisible = true) where T : MonoBehaviour
    {
        Collider2D[] possibleTargets = Physics2D.OverlapCircleAll(position, radius);
        RaycastHit2D[] hits;
        T target = null;

        foreach (Collider2D collider in possibleTargets)
        {
            if (collider.isTrigger && collider.GetComponentInParent<T>() != null)
            {
                bool isVisible = true;
                Vector2 targetDirection = collider.transform.parent.position - position;
                
                if (!inSector || Vector2.Angle(attackDirection, targetDirection) <= 45)
                {
                    if (target == null || (target.transform.position - position).magnitude > targetDirection.magnitude)
                    {
                        hits = Physics2D.RaycastAll(position, targetDirection.normalized, targetDirection.magnitude);
                        foreach (RaycastHit2D hit in hits)
                        {
                            if (hit.transform.tag == "Wall")
                            {
                                isVisible = false;
                                break;
                            }
                        }

                        if (isVisible || !mustBeVisible) target = collider.GetComponentInParent<T>();
                    }
                }
            }
        }

        return target;
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
    private void DealDamage(IAttackObject attackObject, IDefenseObject defenseObject, AttackParameters attack, bool isPerFrame = false)
    {
        float multiplier = attack.multiplier;
        if (isPerFrame)
        {
            multiplier = Time.deltaTime;
        }

        float damage = attack.damage;
        damage *= 1.0f + (attackObject.GetAttackValue(attack.attackType) - defenseObject.GetDefenseValue(attack.attackType)) * 0.05f;

        if (damage < 1.0f)
        {
            damage = 1.0f;
        }
        
        string text = defenseObject.DealDamage(damage * multiplier).ToString("F1");
        GameObject textObject = Instantiate(battleData.textDamagePrefab, Camera.main.WorldToScreenPoint(defenseObject.GetPosition() + Vector3.up * 1.6f), Quaternion.identity, GameObject.FindGameObjectWithTag("MainCanvas").transform);
        if (attack.multiplier < 1)
        {
            textObject.GetComponent<TMP_Text>().color = Color.grey;
        }
        else if (attack.multiplier == 1)
        {
            textObject.GetComponent<TMP_Text>().color = Color.yellow;
        }
        else if (attack.multiplier > 1)
        {
            textObject.GetComponent<TMP_Text>().color = Color.red;
        }
        textObject.GetComponent<TMP_Text>().text = text;
        textObject.GetComponent<DamageLabel>().SetDefenseObject(defenseObject);
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
            enemyParameters.isAttacking = false;
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

    public void AssingAbility(PlayerData playerData, AttackParameters attack, AttackButton attackButton)
    {
        if (playerData.attacks == null)
        {
            playerData.attacks = CloneDictionary(playerAttacks);
        }

        //Find current AttackButton and AttackParameters
        AttackButton currentAttackButton = AttackButton.NONE;
        foreach (KeyValuePair<AttackButton, AttackParameters> pair in playerData.attacks[playerData.type])
        {
            if (pair.Value.attackName.Equals(attack.attackName))
            {
                currentAttackButton = pair.Key;
                break;
            }
        }
        playerData.attacks[playerData.type].TryGetValue(attackButton, out AttackParameters currentAttack);

        if (currentAttackButton == AttackButton.NONE && currentAttack == null)//New key in the dictionary
        {
            playerData.attacks[playerData.type].Add(attackButton, Instantiate(attack));
        }
        else if (currentAttackButton != AttackButton.NONE && currentAttack == null)//Remove key in the dictionary
        {
            playerData.attacks[playerData.type].Remove(currentAttackButton);
            if (attackButton != AttackButton.NONE)
            {
                playerData.attacks[playerData.type][attackButton] = Instantiate(attack);
            }
        }
        else if (currentAttack != null)//Swap the attacks
        {
            playerData.attacks[playerData.type][attackButton] = Instantiate(attack);
            if (currentAttackButton != AttackButton.NONE)
            {
                playerData.attacks[playerData.type][currentAttackButton] = currentAttack;
            }
        }
    }

    private void PlayerUseSword(PlayerData playerData, AttackParameters attack)
    {
        bool shakeCamera = false;
        bool stopAnimation = false;
        if (playerData.critChance >= UnityEngine.Random.Range(0f, 100f))
        {
            attack.multiplier = playerData.critMultiplier;
        }
        else
        {
            attack.multiplier = 1.0f;
        }

        List<EnemyController> enemies = DetectTargets<EnemyController>(playerData.position, attack.range + playerData.colliderRadius, playerData.attackDirection);
        foreach (EnemyController enemy in enemies)
        {
            if (enemy.IsAlive())
            {
                shakeCamera = true;
                DealDamage(playerData, enemy.enemyParameters, attack);
            }
            if (enemy.enemyParameters.type == EnemyParameters.EnemyType.GUARD)
            {
                Instantiate(battleData.Sparks, enemy.transform.position + new Vector3(0f, 0.75f), Quaternion.identity);
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

    private void PlayerBlock(PlayerData playerData, AttackParameters attack)
    {
        playerData.isBlocking = true;
        attack.playerData = playerData;
        attack.endDelegate = PlayerBlockEnd;
        StartCoroutine(StartAttack(attack));
        runningAttacks.Add(attack);
    }

    private void PlayerBlockEnd(AttackParameters attack)
    {
        attack.playerData.isBlocking = false;
    }

    private void PlayerActivateShield(PlayerData playerData, AttackParameters attack)
    {
        GameObject shield = Instantiate(battleData.shieldPrefab, playerData.transform);
        shield.transform.localPosition = new Vector3(0, 0.5f, 0);
        shield.GetComponent<ParticleSystem>().Play(battleData.shieldPrefab);
        battleData.shieldsByCreatures.Add(playerData, shield);

        playerData.defense *= 5;
        playerData.specialDefense *= 5;

        attack.playerData = playerData;

        attack.endDelegate = PlayerDeactivateShield;
        StartCoroutine(StartAttack(attack));
        runningAttacks.Add(attack);
    }

    private void PlayerDeactivateShield(AttackParameters attack)
    {
        GameObject shield = battleData.shieldsByCreatures[attack.playerData];
        if (shield != null)
        {
            shield.GetComponent<ParticleSystem>().Stop();
            Destroy(shield, 0.5f);
            battleData.shieldsByCreatures.Remove(attack.playerData);
        }

        attack.playerData.defense /= 5;
        attack.playerData.specialDefense /= 5;
    }

    private void PlayerLeap(PlayerData playerData, AttackParameters attack)
    {
        attack.playerData = playerData;
        
        EnemyController enemy = GetNearestTarget<EnemyController>(playerData.position, attack.range + playerData.colliderRadius, playerData.attackDirection, false);
        if (enemy != null)
        {
            attack.enemyParameters = enemy.enemyParameters;
        }
        else
        {
            attack.enemyParameters = null;
        }

        attack.runningDelegate = PlayerLeapRunning;
        attack.endDelegate = PlayerLeapEnd;
        StartCoroutine(StartAttack(attack));
        runningAttacks.Add(attack);
    }

    private void PlayerLeapRunning(AttackParameters attack)
    {
        if (attack.enemyParameters == null)
        {
            attack.playerData.transform.GetComponent<Rigidbody2D>().velocity = attack.playerData.attackDirection.normalized * attack.range / attack.duration;
        }
        else
        {
            attack.playerData.attackDirection = (attack.enemyParameters.position - attack.playerData.position).normalized;
            attack.playerData.transform.GetComponent<Rigidbody2D>().velocity = (attack.enemyParameters.position - attack.playerData.position) / attack.duration;
        }
    }

    private void PlayerLeapEnd(AttackParameters attack)
    {
        EnemyController enemy = GetNearestTarget<EnemyController>(attack.playerData.position, attack.playerData.colliderRadius + 0.25f, attack.playerData.attackDirection, false);
        if (enemy != null)
        {
            Camera.main.GetComponent<Animator>().SetTrigger("shake");
            DealDamage(attack.playerData, enemy.enemyParameters, attack);
        }
    }

    private void PlayerBoomerang(PlayerData playerData, AttackParameters attack)
    {
        attack.playerData = playerData;
        GameObject boomerang = Instantiate(battleData.boomerangPrefab, playerData.position, Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.up, playerData.attackDirection, Vector3.forward), Vector3.forward));
        boomerang.GetComponent<ProjectileController>().Launch("Player", playerData, attack, playerData.attackDirection);
    }

    private void PlayerDash(PlayerData playerData, AttackParameters attack)
    {
        battleData.attackDirections.Add(playerData, playerData.attackDirection);
        playerData.transform.GetComponent<Rigidbody2D>().excludeLayers = LayerMask.GetMask(new string[] { "Creatures" });

        attack.playerData = playerData;
        attack.runningDelegate = PlayerDashRunning;
        attack.endDelegate = PlayerDashEnd;
        StartCoroutine(StartAttack(attack));
        runningAttacks.Add(attack);
    }

    private void PlayerDashRunning(AttackParameters attack)
    {
        attack.playerData.attackDirection = battleData.attackDirections[attack.playerData].normalized;
        attack.playerData.transform.GetComponent<Rigidbody2D>().velocity = battleData.attackDirections[attack.playerData].normalized * attack.range / attack.duration;

        List<EnemyController> enemies = DetectTargets<EnemyController>(attack.playerData.position, attack.playerData.colliderRadius + 0.25f, attack.playerData.attackDirection, false);
        foreach (EnemyController enemy in enemies)
        {
            if (enemy.IsAlive())
            {
                DealDamage(attack.playerData, enemy.enemyParameters, attack, true);
            }
        }

        List<ObjectController> breakables = DetectTargets<ObjectController>(attack.playerData.position, attack.playerData.colliderRadius * 2, attack.playerData.attackDirection, false);
        foreach (ObjectController breakable in breakables)
        {
            if (breakable.IsIntact())
            {
                DealDamage(attack.playerData, breakable.objectParameters, attack, true);
            }
        }
    }

    private void PlayerDashEnd(AttackParameters attack)
    {
        attack.playerData.transform.GetComponent<Rigidbody2D>().excludeLayers = LayerMask.GetMask(new string[] { "Nothing" });
        battleData.attackDirections.Remove(attack.playerData);
    }

    private void PlayerExplosionClosest(PlayerData playerData, AttackParameters attack)
    {
        EnemyController enemy = GetNearestTarget<EnemyController>(playerData.position, attack.range + playerData.colliderRadius, playerData.attackDirection, false, false);
        if (enemy != null)
        {
            Instantiate(battleData.explosionPrefab, enemy.transform.position - new Vector3(0, enemy.enemyParameters.colliderRadius/2, 0), Quaternion.identity).GetComponent<ProjectileController>().Launch(playerData, attack);
        }
    }

    private void PlayerPushingWave(PlayerData playerData, AttackParameters attack)
    {
        attack.enemyParametersList = new List<EnemyParameters>();
        List<EnemyController> enemies = DetectTargets<EnemyController>(playerData.position, attack.range + playerData.colliderRadius, playerData.attackDirection, false);
        foreach (EnemyController enemy in enemies)
        {
            if (enemy.IsAlive())
            {
                attack.enemyParametersList.Add(enemy.enemyParameters);
            }
        }

        attack.playerData = playerData;
        attack.runningDelegate = PlayerPushingWaveRunning;
        attack.endDelegate = PlayerPushingWaveEnd;
        StartCoroutine(StartAttack(attack));
        runningAttacks.Add(attack);
    }

    private void PlayerPushingWaveRunning(AttackParameters attack)
    {
        foreach (EnemyParameters enemy in attack.enemyParametersList)
        {
            enemy.transform.GetComponent<Rigidbody2D>().velocity = (enemy.position - attack.playerData.position).normalized * 8.0f;
        }
    }

    private void PlayerPushingWaveEnd(AttackParameters attack)
    {
        foreach (EnemyParameters enemy in attack.enemyParametersList)
        {
            enemy.transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    private void PlayerGuisonKnife(PlayerData playerData, AttackParameters attack)
    {
        battleData.guisonKnifes = new List<ProjectileController>();
        for (int i = 0; i < 5; i++)
        {
            ProjectileController projectile = Instantiate(battleData.guisonKnifePrefab, 
                playerData.position, 
                Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.up, playerData.attackDirection, Vector3.forward) -45 + 22.5f * i, Vector3.forward)).GetComponent<ProjectileController>();
            projectile.Launch("Player", playerData, attack, projectile.transform.up);
            projectile.Launch(projectile.transform.up * attack.range * 3.0f / attack.duration);
            battleData.guisonKnifes.Add(projectile);
        }

        attack.playerData = playerData;
        attack.runningDelegate = PlayerGuisonKnifeRunning;
        attack.endDelegate = PlayerGuisonKnifeEnd;
        StartCoroutine(StartAttack(attack));
        runningAttacks.Add(attack);
    }

    private void PlayerGuisonKnifeRunning(AttackParameters attack)
    {
        foreach (ProjectileController projectile in battleData.guisonKnifes)
        {
            if (projectile.timer >= attack.duration / 3.0f)
            {
                projectile.Launch(Vector3.zero);
            }
            if (projectile.timer >= attack.duration * 2 / 3.0f && projectile.timer < attack.duration)
            {
                projectile.Launch((attack.playerData.position - projectile.transform.position) / (attack.duration - projectile.timer));
            }
        }
    }

    private void PlayerGuisonKnifeEnd(AttackParameters attack)
    {
        foreach (ProjectileController projectile in battleData.guisonKnifes)
        {
            Destroy(projectile.gameObject);
        }
        battleData.guisonKnifes.Clear();
    }

    private void PlayerMindControl(PlayerData playerData, AttackParameters attack)
    {

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
        attack.endDelegate = DisableStun;

        StartCoroutine(StartAttack(attack));
        runningAttacks.Add(attack);
    }

    private void GhostShoot(EnemyParameters enemyParameters, AttackParameters attack)
    {
        ProjectileController projectileController = Instantiate(enemyParameters.projectilePrefab, enemyParameters.position, new Quaternion()).GetComponent<ProjectileController>();
        projectileController.Launch("Enemy", enemyParameters, attack, enemyParameters.attackDirection);
    }

    private void DisableStun(AttackParameters attack)
    {
        if (attack.playerData != null) attack.playerData.DisableStun();
        if (attack.enemyParameters != null) attack.enemyParameters.DisableStun();
    }
}
