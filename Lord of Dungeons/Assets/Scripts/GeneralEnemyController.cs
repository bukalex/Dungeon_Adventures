using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralEnemyController : MonoBehaviour
{
    private EnemyAnimationController animationController;

    void Start()
    {
        animationController = GetComponentInChildren<EnemyAnimationController>();
    }

    void Update()
    {
        
    }

    //Call this method when player hits enemy
    public void OnHit()
    {
        animationController.Hurt();
    }
}
