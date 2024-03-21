using UnityEngine;

public class TrapController : MonoBehaviour
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



    private Vector3 lastPlayerPosition;
    private Vector3 movementDirection = Vector3.zero;
    private float targetDistance;
    private bool alreadyDead = false;

    private int spriteOrder;
    private int enemyOrder;
    private float angle;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (BattleManager.Instance.EnemyPerformAction(enemyParameters, BattleManager.AttackButton.LMB))
            {
                Debug.Log("Hit you.");
            }
        }
    }

    private void Update()
    {

    }
}
