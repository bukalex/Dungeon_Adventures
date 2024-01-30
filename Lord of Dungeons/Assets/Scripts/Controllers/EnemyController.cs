using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private EnemyParameters enemyParameters;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Rigidbody2D body;

    [SerializeField]
    private CircleCollider2D circleCollider;

    private DirectionName directionName = DirectionName.FRONT;
    private Vector3 movementDirection = Vector3.zero;
    private Vector3 attackDirection = Vector3.down;
    private float targetDistance;

    private float health;
    private float mana;
    private float stamina;

    private bool interruptAnimation = false;
    private bool isReadyToAttack = true;

    public enum DirectionName { FRONT, BACK, LEFT, RIGHT }

    void Awake()
    {
        animator.runtimeAnimatorController = enemyParameters.animController;

        health = enemyParameters.health;
        mana = enemyParameters.mana;
        stamina = enemyParameters.stamina;
    }

    void Update()
    {
        if (isAlive())
        {
            //Movement and attack
            if (PlayerDetected())
            {
                ChangeDirection(Vector2.SignedAngle(Vector3.right, enemyParameters.playerData.position - transform.position));

                if (targetDistance > enemyParameters.attackRange)
                {
                    Seek();
                }
                else
                {
                    if (isReadyToAttack)
                    {
                        PerformAttack();
                        StartCoroutine(Cooldown(enemyParameters.attackCooldown));
                    }

                    body.velocity = Vector3.zero;
                }
            }
        }
        else
        {
            circleCollider.enabled = false;
            body.velocity = Vector3.zero;
        }
    }

    private void Seek()
    {

    }

    private void PerformAttack()
    {
        Attack();

        switch (enemyParameters.type)
        {
            case EnemyParameters.EnemyType.GUARD:
                enemyParameters.playerData.DealDamage(enemyParameters.attackType, enemyParameters.attack);
                break;

            case EnemyParameters.EnemyType.GHOST:
                break;

            case EnemyParameters.EnemyType.RAT:
                break;

            case EnemyParameters.EnemyType.BAT:
                break;
        }
    }

    private bool PlayerDetected()
    {
        targetDistance = (enemyParameters.playerData.position - transform.position).magnitude - enemyParameters.playerData.colliderRadius - enemyParameters.colliderRadius;
        return targetDistance <= enemyParameters.detectionRadius && enemyParameters.playerData.isAlive();
    }

    public bool isAlive()
    {
        return health > 0;
    }

    public void DealDamage(PlayerData.AttackType attackType, float damage)
    {
        Debug.Log("Enemy was hit. Damage: " + damage);

        health -= damage;
        if (!isAlive())
        {
            Die();
        }
    }

    IEnumerator Cooldown(float time)
    {
        isReadyToAttack = false;
        yield return new WaitForSeconds(time);
        isReadyToAttack = true;
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
        if (!interruptAnimation)
        {
            Stop();
            animator.SetTrigger("attack");
        }
    }

    private void Die()
    {
        StartCoroutine(Interrupt());
        Stop();
        animator.SetTrigger("isDead");
    }

    IEnumerator Interrupt()
    {
        interruptAnimation = true;
        yield return new WaitForSeconds(1.0f);
        interruptAnimation = false;
    }
    #endregion
}
