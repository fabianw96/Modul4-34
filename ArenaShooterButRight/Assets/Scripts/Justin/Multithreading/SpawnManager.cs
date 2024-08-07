using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform player;
    public int capsuleCount = 10000;

    void Start()
    {
        SpawnCapsules();
    }

    void SpawnCapsules()
    {
        for (int i = 0; i < capsuleCount; i++)
        {
            Vector3 position = new Vector3(Random.Range(-50, 50), 0.5f, Random.Range(-50, 50));
            Instantiate(enemyPrefab, position, Quaternion.identity);
        }
    }
}
