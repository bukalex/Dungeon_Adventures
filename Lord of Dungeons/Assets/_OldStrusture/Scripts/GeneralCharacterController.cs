using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralCharacterController : MonoBehaviour
{
    [SerializeField] float speed = 0.75f;
    [SerializeField] public float health = 100.0f;
    [SerializeField] public UIManager uiManager;

    private Rigidbody2D body;
    private Vector2 direction;
    private Vector2 attackDirection = new Vector2(0, -1);
    public healthBar healthBar;

    private PlayerAnimationController animationController;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animationController = GetComponentInChildren<PlayerAnimationController>();

        healthBar.SetMaxHealth(100);
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
        else
        {
            body.velocity = Vector2.zero;
            GetComponent<BoxCollider2D>().enabled = false;
        }

        healthBar.SetHealth(health);

        if (health > 100)
        {
            healthBar.SetHealth(100);
        }
    }

    //Detect enemies in the 90 degree sector in front of the player
    public List<GeneralEnemyController> DetectEnemies(float range, bool inSector = true)
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, range);
        List<GeneralEnemyController> enemies = new List<GeneralEnemyController>();

        foreach (Collider2D target in targets)
        {
            if (target.tag.Equals("Enemy") && target.GetComponent<GeneralEnemyController>().isAlive() && target.GetType() == typeof(PolygonCollider2D))
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

        return enemies;
    }

    //Detect objects in the 90 degree sector in front of the player
    public List<DestroyBehaviour> DetectDestroyObjects(float range, bool inSector = true)
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, range);
        List<DestroyBehaviour> objects = new List<DestroyBehaviour>();

        foreach (Collider2D target in targets)
        {
            if (target.tag.Equals("Object") && target.GetType() == typeof(PolygonCollider2D))
            {
                Vector2 targetDirection = target.transform.position - transform.position;

                if (!inSector)
                {
                    objects.Add(target.GetComponent<DestroyBehaviour>());
                }
                else if (Vector2.Angle(attackDirection, targetDirection) <= 45)
                {
                    objects.Add(target.GetComponent<DestroyBehaviour>());
                }
            }
        }

        return objects;
    }

    //Detect objects in the 90 degree sector in front of the player
    public List<CollectBehaviour> DetectCollectObjects(float range, bool inSector = true)
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, range);
        List<CollectBehaviour> objects = new List<CollectBehaviour>();

        foreach (Collider2D target in targets)
        {
            if (target.tag.Equals("Object") && target.GetType() == typeof(PolygonCollider2D))
            {
                Vector2 targetDirection = target.transform.position - transform.position;

                if (!inSector)
                {
                    objects.Add(target.GetComponent<CollectBehaviour>());
                }
                else if (Vector2.Angle(attackDirection, targetDirection) <= 45)
                {
                    objects.Add(target.GetComponent<CollectBehaviour>());
                }
            }
        }

        return objects;
    }

    //Call this method when enemy hits player
    public void DealDamage(float damage)
    {
        Debug.Log("Character was hit");

        health -= damage;

        healthBar.SetHealth(health);
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
