using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralCharacterController : MonoBehaviour
{
    [SerializeField] Rigidbody2D body;

    private Vector2 direction;
    private float speed = 0.75f;

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
    }
}
