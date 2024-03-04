using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
    public float timer;
    private Vector2 direction;
    private float targetAngle;
    private float angleDifference;
    private float rotationStep;
    private float rotationAmount;

    public enum ProjectileType { BULLET, BOOMERANG, GUISON_KNIFE, PARTICLES }

    void Update()
    {
        if (type == ProjectileType.BOOMERANG)
        {
            timer += Time.deltaTime;
            transform.GetChild(0).Rotate(0, 0, 1500 * Time.deltaTime);
            Seek();
        }

        if (type == ProjectileType.GUISON_KNIFE)
        {
            timer += Time.deltaTime;
        }
    }

    private void Seek()
    {
        direction = (attack.playerData.position + new Vector3(0.1f, 0.1f, 0) - transform.position).normalized;
        targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;

        rotationStep = 200 * Time.deltaTime;
        angleDifference = Mathf.DeltaAngle(targetAngle, transform.eulerAngles.z);
        rotationAmount = Mathf.Clamp(angleDifference, -rotationStep, rotationStep);
        
        transform.Rotate(Vector3.forward, rotationAmount);
        body.velocity = transform.up * speed;
    }

    public void Launch(IAttackObject attackObject, AttackParameters attack)
    {
        this.attackObject = attackObject;
        this.attack = attack;
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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.isTrigger)
        {
            if (collision.transform.parent == null) return; 
            string targetTag = collision.transform.parent.tag;
            
            if (type == ProjectileType.PARTICLES || 
                parentTag == "Enemy" && targetTag == "Player" ||
                parentTag == "Player" && (targetTag == "Enemy" || targetTag == "Object"))
            {
                BattleManager.Instance.ProjectileHit(attackObject, collision.GetComponentInParent<IDefensiveMonoBehaviour>().GetDefenseObject(), attack);
            }

            if (type != ProjectileType.BOOMERANG &&
                type != ProjectileType.GUISON_KNIFE &&
                type != ProjectileType.PARTICLES &&
                !targetTag.Equals(parentTag))
            {
                Destroy(gameObject);
            }

            if (type == ProjectileType.BOOMERANG && targetTag.Equals(parentTag) && timer > attack.duration * 0.5f)
            {
                Destroy(gameObject);
            }
        }
        else if (type != ProjectileType.BOOMERANG &&
            type != ProjectileType.GUISON_KNIFE &&
            type != ProjectileType.PARTICLES && 
            !collision.transform.tag.Equals("Enemy") &&
            !collision.transform.tag.Equals("Player"))
        {
            Destroy(gameObject);
        }
    }
}
