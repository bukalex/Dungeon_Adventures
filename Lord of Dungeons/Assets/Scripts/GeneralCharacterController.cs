using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GeneralCharacterController : MonoBehaviour
{
    [SerializeField] float speed = 0.75f;
    [SerializeField] float health = 100.0f;

    private Rigidbody2D body;
    private Vector2 direction;
    private Vector2 attackDirection = new Vector2(0, -1);

    private PlayerAnimationController animationController;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animationController = GetComponentInChildren<PlayerAnimationController>();
    }

    void Update()
    {
        if (isAlive())
        {
            //Movement with WASD and run with left shift
            //Cannot run and fight at once
            direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            direction.Normalize();

            if (Input.GetKey(KeyCode.LeftShift) && !Input.GetMouseButton(0) && !Input.GetMouseButton(1))
            {
                body.velocity = direction * speed * 2;
            }
            else
            {
                body.velocity = direction * speed;
            }

            //Save attackDirection
            if (direction.magnitude != 0)
            {
                attackDirection = direction;
            }
        }
    }

    //Detect enemies in the 90 degree sector in front of the player
    public void DetectEnemies(float range, out List<GeneralEnemyController> enemies, bool inSector = true)
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, range);
        enemies = new List<GeneralEnemyController>();
        

        foreach (Collider2D target in targets)
        {
            //Detects enemies
            if (target.CompareTag("Enemy") && target.GetComponent<GeneralEnemyController>().isAlive() && target.GetType() == typeof(PolygonCollider2D))
            {
                Vector2 targetDirection = target.transform.position - transform.position;

                if (!inSector)
                {
                    enemies.Add(target.GetComponent<GeneralEnemyController>());
                }
                else if (Vector2.Angle(attackDirection, targetDirection) <= 45)
                {
                    enemies.Add(target.GetComponent<GeneralEnemyController>());
                }
            }
        }
    }

    public void DetectObjects(float range, out List<GameObject> objects, bool inSector = true)
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, range);
        objects = new List<GameObject>();

        foreach (Collider2D target in targets)
        {
            //Detects objects
            if (target.CompareTag("Object"))
            {
                Vector2 targetDirection = target.transform.position - transform.position;

                if (!inSector)
                {
                    objects.Add(target.gameObject);
                }
                else if (Vector2.Angle(attackDirection, targetDirection) <= 45)
                {
                    objects.Add(target.gameObject);
                }
            }
        }
    }

    //Call this method when enemy hits player
    public void DealDamage(float damage)
    {
        Debug.Log("Character was hit");

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
