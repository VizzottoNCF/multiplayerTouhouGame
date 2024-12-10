using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    private Vector2 moveDirection;
    [SerializeField] public float speed = 1f;

    private float life = 0f;
    private void Start()
    {
        if (GetComponent<NetworkObject>() != null && NetworkManager.Singleton.IsServer)
        {
            Invoke("rf_Destroy", 3f);
            rf_EnableNetworkBulletServerRPC();
        }
    }

    private void Update()
    {
        life += Time.deltaTime;
        if (life > 3f) { rf_DisableNetworkBulletServerRPC(); }
    }

    /// <summary> Advances bullet </summary>
    private void FixedUpdate() { this.transform.Translate(moveDirection * speed * Time.deltaTime); }

    /// <summary> Gets direction set by emitter </summary>
    public void rf_SetMoveDirection(Vector2 dir) { moveDirection = dir; }

    private void rf_Destroy() { rf_DisableNetworkBulletServerRPC(); }

    private void OnDisable() { CancelInvoke(); }

    /// <summary> Take damage when hit by bullet </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // check if either player bullet or enemy bullet, then check for target, reduce hp and give iframes
        // rf_TakeDamage() param == iframe quantity
        if (gameObject.CompareTag("PlayerBullet"))
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                collision.GetComponent<EnemyEntity>().rf_TakeDamage(10);

                // deactivate bullet if it damages
                //gameObject.SetActive(false);
                rf_DisableNetworkBulletServerRPC();
            }
        }
        
        if (gameObject.CompareTag("Bullet"))
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                collision.GetComponent<EnemyEntity>().rf_TakeDamage(15);
            }
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void rf_DisableNetworkBulletServerRPC()
    {
        //if (gameObject.CompareTag("PlayerBullet")) { gameObject.GetComponent<NetworkObject>().Despawn(); Destroy(gameObject); }
        //else { gameObject.GetComponent<NetworkObject>().Despawn(); gameObject.SetActive(false); }
        if (gameObject.CompareTag("PlayerBullet"))
        {
            if (!gameObject.GetComponent<NetworkObject>().IsSpawned)
            {
                gameObject.GetComponent<NetworkObject>().Despawn(destroy: true); Destroy(gameObject);
            }
        }
        else { 
            //gameObject.GetComponent<NetworkObject>().Despawn(destroy: false); 
            gameObject.SetActive(false); }
    }


    [ServerRpc]
    private void rf_EnableNetworkBulletServerRPC()
    {
        if (!GetComponent<NetworkObject>().IsSpawned) { GetComponent<NetworkObject>().Spawn(); }
        life = 0f;
    }

}
