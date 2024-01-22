using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorController : MonoBehaviour
{
    [SerializeField] Rigidbody2D body;

    private Vector2 direction;
    private float speed = 0.75f;

    void Update()
    {
        //Movement with WASD
        direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        direction.Normalize();
        body.velocity = direction * speed;
    }
}
