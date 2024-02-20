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
    private float timer;
    private float radiusSpeed;

    void Start()
    {
        initialPosition = transform.position;
    }

    public enum ProjectileType { BULLET, BOOMERANG }

    void Update()
    {
        if (type == ProjectileType.BOOMERANG)
        {
            timer += Time.deltaTime;
            body.velocity = Mathf.Sign(-timer) * transform.right * radiusSpeed;
            transform.RotateAround(initialPosition, Vector3.forward, speed * Time.deltaTime);
        }
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

            if (type != ProjectileType.BOOMERANG && !targetTag.Equals(parentTag))
            {
                Destroy(gameObject);
            }
        }
        else if (type != ProjectileType.BOOMERANG && !collision.transform.tag.Equals(parentTag))
        {
            Destroy(gameObject);
        }
    }
}
