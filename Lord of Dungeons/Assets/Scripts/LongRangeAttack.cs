using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class LongRangeAttack : MonoBehaviour
{
    public GameObject bulletPrefab;
    public void Shoot()
    {
        //Getting mouse poisition
        Vector3 mousPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousPos.z = 0f;

        mousPos = mousPos.normalized;

        Vector3 direction2target = mousPos - transform.position;

        Instantiate(bulletPrefab, direction2target, direction2target.rotation);
    }
}
