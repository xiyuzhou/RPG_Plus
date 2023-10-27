using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class EnemySpawner : MonoBehaviourPun
{
    public string enemyPrefabPath;
    public float maxEnemies;
    public float spawnRadius;
    public float spawnCheckTime;
    private float lastSpawnCheckTime;
    public float enemySpawnRadius;
    private List<GameObject> curEnemies = new List<GameObject>();
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (Time.time - lastSpawnCheckTime > spawnCheckTime)
        {
            lastSpawnCheckTime = Time.time;
            TrySpawn();
        }
    }
    void TrySpawn()
    {
        // remove any dead enemies from the curEnemies list
        for (int x = 0; x < curEnemies.Count; ++x)
        {
            if (!curEnemies[x])
                curEnemies.RemoveAt(x);
        }

        // if we have maxed out our enemies, return
        if (curEnemies.Count >= maxEnemies)
            return;

        // Otherwise, spawn an enemy at a valid location
        Vector3 randomInCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPoint = transform.position + randomInCircle;

        // Check for collisions with 2D colliders at the spawn point
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(spawnPoint, enemySpawnRadius);

        // Ensure the spawn point is not colliding with any 2D colliders
        if (hitColliders.Length == 0)
        {
            GameObject enemy = PhotonNetwork.Instantiate(enemyPrefabPath, spawnPoint, Quaternion.identity);
            curEnemies.Add(enemy);
        }
        else
        {
            // Handle the case where a collision was detected (e.g., choose a different spawn point or take other action)
            // You may want to add additional logic here, like finding a new spawn point.
        }
    }



}
