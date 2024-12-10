using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
[System.Serializable]
public class Wave
{
    public string waveName;
    public int noOfEnemies;

    public GameObject[] typeOfEnemies;

    public float spawnInterval;
}


public class WaveSpawner : NetworkBehaviour
{
    [SerializeField] Transform[] spawnPositions;
    [SerializeField] Wave[] waves;

    private Wave currentWave;
    private int currentWaveNumber = 0;
    private float nextSpawnTime = 0;

    public bool canSpawn = false;

    private void Update()
    {
        if (!IsServer) { return; }

        // once wave has started, it'll start looking if all enemies are deactivated (meaning they're dead)
        currentWave = waves[currentWaveNumber];

        rf_SpawnWave();

        GameObject[] totalEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (totalEnemies.Length == 0 && !canSpawn && currentWaveNumber+1 != waves.Length) { rf_SpawnNextWave(); }
    }

    private void rf_SpawnNextWave()
    {
        currentWaveNumber++;
        canSpawn = true;
    }

    public void rf_SpawnWave()
    {
        if (canSpawn && nextSpawnTime < Time.time)
        {
            GameObject randomEnemy = currentWave.typeOfEnemies[Random.Range(0, currentWave.typeOfEnemies.Length)];
            Transform randomPoint = spawnPositions[Random.Range(0, spawnPositions.Length)];

            GameObject enemyInst = Instantiate(randomEnemy, randomPoint.position, Quaternion.identity);
            NetworkObject netObj = enemyInst.GetComponent<NetworkObject>();

            if (enemyInst != null) { if (netObj != null && !netObj.IsSpawned) { netObj.Spawn(true); }}

            nextSpawnTime = Time.time + currentWave.spawnInterval;
            currentWave.noOfEnemies--;
            if (currentWave.noOfEnemies == 0) { canSpawn = false; }
        }
    }
}
