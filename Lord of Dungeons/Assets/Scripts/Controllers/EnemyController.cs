using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private EnemyParameters enemyParametersOriginal;
    public EnemyParameters enemyParameters { get; private set; }

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Rigidbody2D body;

    [SerializeField]
    private CapsuleCollider2D capsuleCollider;

    [SerializeField]
    private healthBar HealthBar;

    private DirectionName directionName = DirectionName.FRONT;
    private Vector3 movementDirection = Vector3.zero;
    private float targetDistance;
    private bool alreadyDead = false;

    public enum DirectionName { FRONT, BACK, LEFT, RIGHT }

    void Awake()
    {
        enemyParameters = Instantiate(enemyParametersOriginal);

        animator.runtimeAnimatorController = enemyParameters.animController;
        HealthBar.SetMaxHealth(enemyParameters.maxHealth);
    }

    void Update()
    {
        enemyParameters.position = transform.position;
        enemyParameters.attackDirection = (enemyParameters.playerData.position - transform.position).normalized;
        enemyParameters.isUsingMana = false;
        enemyParameters.isUsingStamina = false;

        if (IsAlive() && !enemyParameters.isStunned)
        {
            //Movement and attack
            if (PlayerDetected())
            {
                ChangeDirection(Vector2.SignedAngle(Vector3.right, enemyParameters.playerData.position - transform.position));

                if (targetDistance > BattleManager.Instance.GetAttackRange(enemyParameters.type, BattleManager.AttackButton.LMB))
                {
                    Run();
                    Seek();
                    AvoidObstacles();
                }
                else
                {
                    if (enemyParameters.type == EnemyParameters.EnemyType.GHOST && targetDistance < BattleManager.Instance.GetAttackRange(enemyParameters.type, BattleManager.AttackButton.LMB) * 0.75f)
                    {
                        Run();
                        Flee();
                        AvoidObstacles();
                    }
                    else
                    {
                        body.velocity = Vector3.zero;
                    }

                    if (BattleManager.Instance.EnemyPerformLMB(enemyParameters))
                    {
                        Attack();
                    }
                }

                if (enemyParameters.isBoss && enemyParameters.health <= enemyParameters.maxHealth * 0.5f && targetDistance <= BattleManager.Instance.GetAttackRange(enemyParameters.type, BattleManager.AttackButton.RMB))
                {
                    if (BattleManager.Instance.EnemyPerformRMB(enemyParameters))
                    {
                        Debug.Log("4");
                        Attack();
                    }
                }
            }
            else
            {
                Stop();
                body.velocity = Vector3.zero;
            }

            //Stats restore
            RestoreStats();
        }
        else if (!alreadyDead)
        {
            capsuleCollider.enabled = false;
            body.velocity = Vector3.zero;
            Die();

            if (enemyParameters.type == EnemyParameters.EnemyType.GHOST)
            {
                StartCoroutine(DelayedDie());
            }

            alreadyDead = true;
        }

        HealthBar.SetHealth(enemyParameters.health);
    }

    private void Seek()
    {
        movementDirection = (enemyParameters.playerData.position - transform.position).normalized;
    }

    private void Flee()
    {
        movementDirection = (transform.position - enemyParameters.playerData.position).normalized;

    }

    private void AvoidObstacles()
    {
        if (CastWhisker(enemyParameters.whiskerAngle) || CastWhisker(enemyParameters.whiskerAngle * 2))
        {
            movementDirection = Quaternion.Euler(0, 0, -enemyParameters.rotationSpeed) * movementDirection.normalized;
        }
        else if (CastWhisker(-enemyParameters.whiskerAngle) || CastWhisker(-enemyParameters.whiskerAngle * 2))
        {
            movementDirection = Quaternion.Euler(0, 0, enemyParameters.rotationSpeed) * movementDirection.normalized;
        }

        body.velocity = movementDirection * enemyParameters.speed;
    }

    private bool CastWhisker(float angle)
    {
        Vector2 whiskerDirection = Quaternion.Euler(0, 0, angle) * movementDirection;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, whiskerDirection, enemyParameters.whiskerLength);
        Debug.DrawRay(transform.position, whiskerDirection, Color.red);
        if (hit.collider != null)
        {
            return true;
        }

        return false;
    }

    private bool PlayerDetected()
    {
        targetDistance = (enemyParameters.playerData.position - transform.position).magnitude - enemyParameters.playerData.colliderRadius - enemyParameters.colliderRadius;
        return targetDistance <= enemyParameters.detectionRadius && enemyParameters.playerData.IsAlive();
    }

    public bool IsAlive()
    {
        return enemyParameters.health > 0;
    }

    private void RestoreStats()
    {
        enemyParameters.health = Mathf.Clamp(enemyParameters.health + enemyParameters.healthRestoreRate * Time.deltaTime, 0, enemyParameters.maxHealth);
        if (!enemyParameters.isUsingMana)
        {
            enemyParameters.mana = Mathf.Clamp(enemyParameters.mana + enemyParameters.manaRestoreRate * Time.deltaTime, 0, enemyParameters.maxMana);
        }
        if (!enemyParameters.isUsingStamina)
        {
            enemyParameters.stamina = Mathf.Clamp(enemyParameters.stamina + enemyParameters.staminaRestoreRate * Time.deltaTime, 0, enemyParameters.maxStamina);
        }
    }

    private IEnumerator DelayedDie()
    {
        yield return new WaitForSeconds(0.6f);
        Destroy(gameObject);
    }

    //Animation
    #region
    //Change movement direction
    private void ChangeDirection(float angle)
    {
        if (Mathf.Abs(angle) > 135 && directionName != DirectionName.LEFT)
        {
            directionName = DirectionName.LEFT;
            animator.SetTrigger("side");
            spriteRenderer.flipX = false;
        }
        else if (135 >= angle && angle > 45 && directionName != DirectionName.BACK)
        {
            directionName = DirectionName.BACK;
            animator.SetTrigger("back");
        }
        else if (-135 <= angle && angle < -45 && directionName != DirectionName.FRONT)
        {
            directionName = DirectionName.FRONT;
            animator.SetTrigger("front");
        }
        else if (Mathf.Abs(angle) <= 45 && directionName != DirectionName.RIGHT)
        {
            directionName = DirectionName.RIGHT;
            animator.SetTrigger("side");
            spriteRenderer.flipX = true;
        }
    }

    private void Stop()
    {
        animator.SetBool("isRunning", false);
        animator.SetBool("isWalking", false);
        animator.SetBool("isConfused", false);
    }

    private void Walk()
    {
        animator.SetBool("isWalking", true);
        animator.SetBool("isRunning", false);
    }

    private void Run()
    {
        animator.SetBool("isWalking", true);
        animator.SetBool("isRunning", true);
    }

    private void Attack()
    {
        Stop();
        animator.SetTrigger("attack");
    }

    private void Die()
    {
        Stop();
        animator.SetTrigger("isDead");
    }
    #endregion
}
