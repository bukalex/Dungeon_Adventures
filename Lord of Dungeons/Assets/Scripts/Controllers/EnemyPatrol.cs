using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    private bool isPatrolling = true; // Bool to track patrolling state
    public Transform[] patrolArea; // Define patrol area
    private int currentAreaIndex = 0;
    private int patrolSpeed = 1;
    private float patrolAreaReached;

    void Update()
    {
        if (isPatrolling)
        {
            Patrol();
        }
        else
        {
            // Attack player logic from other script
        }
    }
    private void Patrol()
    {
        // Move towards current area
        Vector3 targetPosition = patrolArea[currentAreaIndex].position;
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        transform.Translate(moveDirection * Time.deltaTime * patrolSpeed);

        // Check if reached the current waypoint
        if (Vector2.Distance(transform.position, targetPosition) < patrolAreaReached)
        {
            // Move to the next waypoint
            currentAreaIndex = (currentAreaIndex + 1) % patrolArea.Length;
        }
    }

    // Switch between patrolling and other states / attack player
    public void SetPatrolling(bool value)
    {
        isPatrolling = value;
    }
}
