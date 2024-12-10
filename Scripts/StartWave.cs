using UnityEngine;
using Unity.Netcode;

public class StartWave : NetworkBehaviour
{
    [SerializeField] GameObject waveSpawner;

    private void Update()
    {
        if (!IsServer) { return; }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            waveSpawner.GetComponent<WaveSpawner>().canSpawn = true;
            //waveSpawner.GetComponent<WaveSpawner>().rf_SpawnWave();
            Destroy(gameObject);
        }
    }
}
