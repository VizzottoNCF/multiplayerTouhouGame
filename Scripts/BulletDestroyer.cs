using Unity.Netcode;
using UnityEngine;

public class BulletDestroyer : MonoBehaviour
{
    private Vector2 spawnpoint;
    [SerializeField]
    float timer = 0f;

    [Header("Attributes")]
    [SerializeField] float life = 4f;
    [SerializeField] float speed = 5f;


    private void Start()
    {
        spawnpoint = new Vector2(transform.position.x, transform.position.y);
    }

    private void Update()
    {
        // kills bullet if time since spawns exceeds life
        if (timer > life) { rf_DisableNetworkBulletServerRPC(); }

        if (timer > 2f) { speed -= Time.deltaTime; }

        timer += Time.deltaTime;

        // calls movement function
        transform.position = rf_Movement(timer);

    }

    private Vector2 rf_Movement(float timer)
    {
        // moves bullet ahead as by it's rotation
        float y = timer * speed * transform.up.y;
        return new Vector2(spawnpoint.x, y + spawnpoint.y); // returns new position
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            Debug.Log("COLIDIU!");
            NetworkObject netObj = other.GetComponent<NetworkObject>();
            if (netObj != null && netObj.IsSpawned) { rf_KillEnemyBulletsServerRPC(netObj.NetworkObjectId); }
        }
    }

    [ServerRpc]
    private void rf_KillEnemyBulletsServerRPC(ulong networkObjectId)
    {
        Debug.Log("Entrou SERVER");
        NetworkObject netObj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
        if (netObj != null) { netObj.gameObject.SetActive(false); } else { Debug.LogWarning("Null"); }
    }

    [ServerRpc]
    private void rf_DisableNetworkBulletServerRPC()
    {
        Destroy(gameObject);
    }
}
