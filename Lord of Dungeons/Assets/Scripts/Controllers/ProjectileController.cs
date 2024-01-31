using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField]
    private float speed = 1.5f;

    [SerializeField]
    private Rigidbody2D body;

    private string parentTag;

    public void Launch(Vector3 direction, string parentTag)
    {
        body.velocity = direction.normalized * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.transform.tag.Equals(parentTag))
        {
            Debug.Log("Hit");
        }
    }
}
