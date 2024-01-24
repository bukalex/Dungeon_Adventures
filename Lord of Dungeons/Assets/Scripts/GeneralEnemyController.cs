using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralEnemyController : MonoBehaviour
{
    [SerializeField] float detectionRange = 5.0f;
    [SerializeField] bool isDetector = false;
    [SerializeField] float speed = 0.3f;
    [SerializeField] float satisfactionRadius = 0.75f;
    [SerializeField] public float health = 100.0f;

    private EnemyAnimationController animationController;
    private Rigidbody2D body;

    public GeneralCharacterController player { get; private set; }

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animationController = GetComponentInChildren<EnemyAnimationController>();
    }

    void Update()
    {
        if (isAlive())
        {
            DetectPlayer(detectionRange, isDetector);

            if (player != null)
            {
                //Change sprite direction
                animationController.ChangeDirection(Vector2.SignedAngle(Vector3.right, player.transform.position - transform.position));
            }
        }
    }

    //Search
    private void DetectPlayer(float range, bool ignoreObstacles)
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, range);
        player = null;

        foreach (Collider2D target in targets)
        {
            if (target.tag.Equals("Player") && target.GetComponent<GeneralCharacterController>().isAlive())
            {
                if (ignoreObstacles)
                {
                    player = target.GetComponent<GeneralCharacterController>();
                }
                else
                {
                    //Check for obstacles
                    player = target.GetComponent<GeneralCharacterController>();
                }
            }
        }
    }

    public void Stop()
    {
        body.velocity = Vector2.zero;
        animationController.Stop();
    }

    //Reach player
    public void RunToPlayer()
    {
        if (player != null)
        {
            Vector2 direction = player.transform.position - transform.position;

            if (direction.magnitude > satisfactionRadius)
            {
                direction.Normalize();
                body.velocity = direction * speed * 2;
                animationController.Run();
            }
            else//Reached player
            {
                Stop();
            }
        }
        else//Lost player
        {
            Stop();
        }
    }

    public bool inAttackRange(float attackRange)
    {
        if ((player.transform.position - transform.position).magnitude <= attackRange)
        {
            return true;
        }

        return false;
    }

    //Call this method when player hits enemy
    public void DealDamage(float damage)
    {
        Debug.Log("Enemy was hit");

        health -= damage;
        if (health <= 0)
        {
            animationController.Die();
        }
    }

    public bool isAlive()
    {
        if (health > 0)
        {
            return true;
        }

        return false;
    }
}
