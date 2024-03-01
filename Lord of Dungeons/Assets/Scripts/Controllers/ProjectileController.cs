using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField]
    private float speed = 1.5f;

    [SerializeField]
    private Rigidbody2D body;

    [SerializeField]
    private ProjectileType type = ProjectileType.BULLET;

    private string parentTag;
    private IAttackObject attackObject;
    private AttackParameters attack;
    private Vector3 initialPosition;
    public float timer;
    private float radiusSpeed;

    void Start()
    {
        initialPosition = transform.position;
    }

    public enum ProjectileType { BULLET, BOOMERANG, GUISON_KNIFE }

    void Update()
    {
        if (type == ProjectileType.BOOMERANG)
        {
            timer += Time.deltaTime;
            body.velocity = Mathf.Sign(-timer) * transform.right * radiusSpeed;
            transform.RotateAround(initialPosition, Vector3.forward, speed * Time.deltaTime);
        }

        if (type == ProjectileType.GUISON_KNIFE)
        {
            timer += Time.deltaTime;
        }
    }

    public void Launch(Vector3 velocity)
    {
        if (velocity.magnitude > 0)
        {
            transform.rotation = Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.up, velocity, Vector3.forward), Vector3.forward);
        }
        else if (body.velocity.magnitude > 0)
        {
            transform.Rotate(-60, 0, 0, Space.Self);
        }
        body.velocity = velocity;
    }

    public void Launch(string parentTag, IAttackObject attackObject, AttackParameters attack, Vector3 direction)
    {
        this.parentTag = parentTag;
        this.attackObject = attackObject;
        this.attack = attack;

        body.velocity = direction.normalized * speed;
        if (type == ProjectileType.BOOMERANG)
        {
            timer = -attack.duration * 0.5f;
            radiusSpeed = attack.range / (attack.duration * 0.5f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger)
        {
            if (collision.transform.parent == null) return; 
            string targetTag = collision.transform.parent.tag;

            if (parentTag == "Enemy" && targetTag == "Player" ||
                parentTag == "Player" && (targetTag == "Enemy" || targetTag == "Object"))
            {
                BattleManager.Instance.ProjectileHit(attackObject, collision.GetComponentInParent<IDefensiveMonoBehaviour>().GetDefenseObject(), attack);
            }

            if (type != ProjectileType.BOOMERANG &&
                type != ProjectileType.GUISON_KNIFE && 
                !targetTag.Equals(parentTag))
            {
                Destroy(gameObject);
            }
        }
        else if (type != ProjectileType.BOOMERANG &&
            type != ProjectileType.GUISON_KNIFE &&
            !collision.transform.tag.Equals(parentTag))
        {
            Destroy(gameObject);
        }
    }
}
