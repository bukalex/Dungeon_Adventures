using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralCharacterController : MonoBehaviour
{
    private Rigidbody2D body;
    private Vector2 direction;
    private Vector2 attackDirection = new Vector2(0, -1);
    private float speed = 0.75f;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
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

    public Vector2 GetAttackDirection()
    {
        return attackDirection;
    }
}
