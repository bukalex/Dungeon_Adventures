using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] public int NumOfBounces = 1;

    private Vector3 lastVelocity;
    private float curSpeed;
    private Vector3 direction;
    private int curBounces = 0;

    private void LateUpdate()
    {
        lastVelocity = rb.velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (curBounces >= NumOfBounces) return;

        curSpeed = lastVelocity.magnitude;
        direction = Vector3.Reflect(lastVelocity.normalized, collision.contacts[0].normal);

        rb.velocity = direction * Mathf.Max(curSpeed, 0);
        curBounces++;
    }
}
