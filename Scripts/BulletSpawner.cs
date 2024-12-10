using UnityEngine;
using Unity.Netcode;
using System.Globalization;

public class BulletSpawner : NetworkBehaviour
{
    public enum SpawnerType { Straight, Spin }

    [Header("Bullet Attributes")]
    /// <summary> ammount to be fired at once </summary>
    [SerializeField] private int bulletsAmmount = 10;
    [SerializeField] private float startAngle = 90f, endAngle = 270f;
    //[SerializeField] private float spawnedBulletSpeed = 5f;
    private Vector2 bulDir;

    [Header("Spawner Attributes")]
    [SerializeField] SpawnerType spawnerType;
    [SerializeField][Range(-2f, 2f)] float firingDirection;
    [SerializeField] private float firingRate = 1f;
    private float timerFire = 0f;

    private void Update()
    {
        if (!IsServer) { return; }
        // timer to continues shooting rounds.
        // using this instead of InvokeRepeating to be able to change bulletfire speed
        timerFire += Time.deltaTime;
        if (timerFire >= firingRate) { rf_FireServerRPC(); timerFire = 0f; }

        
        // if spawner is a spin type, uses euler angle to spin around z axis
        if (spawnerType == BulletSpawner.SpawnerType.Spin) { transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z + firingDirection); }

    }
    [ServerRpc]
    private void rf_FireServerRPC()
    {
        float angleStep = (endAngle - startAngle) / bulletsAmmount;
        float angle = startAngle;

        for (int i = 0; i < bulletsAmmount + 1; i++)
        {
            float bulDirX = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180f);
            float bulDirY = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180f);

            // passes through calculated directions to instantiate a bullet in those coordinates
            Vector3 bulMoveVector = new Vector3(bulDirX, bulDirY, 0f);
            bulDir = (bulMoveVector - transform.position).normalized;


            GameObject bul = ObjectPool.bulletPoolInstance.rf_GetEnemyBullet();
            bul.transform.position = transform.position;
            bul.transform.rotation = transform.rotation;
            bul.GetComponent<Bullet>().rf_SetMoveDirection(bulDir);
            NetworkObject netBul = bul.GetComponent<NetworkObject>();

            if (bul != null)
            { 
                if (netBul != null && !netBul.IsSpawned) { netBul.Spawn(true); } 
                bul.SetActive(true);
            }
            else { Debug.LogError("Falha ao obter bala!"); }


            angle += angleStep;
        }
    }
}
