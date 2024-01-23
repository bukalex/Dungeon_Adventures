using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject guardPrefb;
    [SerializeField] float swarmerInterval = 5.24f;

    void Start()
    {
        StartCoroutine(spawnEnemy(swarmerInterval, guardPrefb));
    }

    private IEnumerator spawnEnemy(float interval, GameObject Enemy)
    {
        yield return new WaitForSeconds(interval);
        float ranX = Random.Range(7, 9);
        float ranY = Random.Range(7, 9);
        GameObject newEnemy = Instantiate(Enemy, new Vector3(ranX, ranY, 0), Quaternion.identity);
        StartCoroutine(spawnEnemy(interval, Enemy));
    }
}

