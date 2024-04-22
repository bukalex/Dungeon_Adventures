using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SolidItem : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Rigidbody2D body;
    [SerializeField]
    private float speed;

    private Item item;
    private bool isCollectable = false;
    private Transform target;
    private Vector2 direction;
    private float targetAngle;
    private float angleDifference;
    private float rotationStep;
    private float rotationAmount;

    public void Initialize(Item item)
    {
        spriteRenderer.sprite = item.image;
        this.item = item;
        StartCoroutine(SetCollectable());
    }

    private IEnumerator SetCollectable()
    {
        yield return new WaitForSeconds(0.7f);
        isCollectable = true;
    }

    private void Seek()
    {
        direction = (target.position - transform.position).normalized;
        body.velocity = direction * speed;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isCollectable && item != null && collision.isTrigger && collision.transform.parent != null && collision.transform.parent.tag == "Player")
        {
            if (InventoryManager.Instance.HasSpace(item))
            {
                target = collision.transform.parent;
                Seek();
                if ((target.position - transform.position).magnitude <= 0.25f)
                {
                    if (InventoryManager.Instance.AddItem(item, InventoryManager.Instance.toolBar.ToList(), InventoryManager.Instance.internalInventorySlots.ToList())) Destroy(gameObject);
                }
            }
        }
    }
}
