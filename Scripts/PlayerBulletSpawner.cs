using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;
using UnityEditor;

public class PlayerBulletSpawner : NetworkBehaviour
{
    public enum SpawnerType { Straight, Spin }

    [Header("Bullet Attributes")]
    /// <summary> ammount to be fired at once </summary>
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] GameObject destroyerPrefab;
    [SerializeField] private int bulletsAmmount = 10;
    [SerializeField] private float startAngle = 90f, endAngle = 90f;
    //[SerializeField] private float spawnedBulletSpeed = 5f;
    private Vector2 bulDir;

    [Header("Spawner Attributes")]
    [SerializeField] SpawnerType spawnerType;
    [SerializeField][Range(-1f, 1f)] float firingDirection;
    [SerializeField] private float firingRate = 1f;
    private float timerFire = 0f;
    private bool holdingFire = false;
    private float holdingFireTimer = 0f;
    private float holdingFireTimerSinceLastUse = 14f;
    private bool playedChargeSFX = false;
    [SerializeField] AudioSource bulletDestroyerSFX;

    private void Update()
    {
        //Debug.Log("My owner is " + OwnerClientId + " (player bullet spawner)");
        if (!IsOwner) { return; }

        // timer to continues shooting rounds.
        // using this instead of InvokeRepeating to be able to change bullet fire speed
        timerFire += Time.deltaTime;
        holdingFireTimerSinceLastUse += Time.deltaTime;


        Transform _BulletOutPoint = transform.Find("BulletOutPoint");

        if (timerFire >= firingRate && !holdingFire) 
        {
            rf_FireServerRPC(_BulletOutPoint.position, _BulletOutPoint.rotation); 
            timerFire = 0f; 
        }

        // hold your constant fire to send forth a big projectile killing bullet
        holdingFire = Input.GetKey(KeyCode.Space);

        // when fire is held for 2 seconds, once it's sustained again, release bullet destroyer
        if (Input.GetKeyUp(KeyCode.Space) && holdingFireTimer >= 2f && holdingFireTimerSinceLastUse >= 15f)
            {
                rf_FireBulletDestroyerServerRPC(_BulletOutPoint.position, _BulletOutPoint.rotation);
            }


        if (holdingFire)
        {
            holdingFireTimer += Time.deltaTime;
            if (holdingFireTimer >= 2f && !playedChargeSFX && holdingFireTimerSinceLastUse >= 15)
            {
                bulletDestroyerSFX.pitch = Random.Range(.95f, 1.05f);
                bulletDestroyerSFX.Play();
                playedChargeSFX = true;
            }
        }
        else
        {
            playedChargeSFX = false;
            holdingFireTimer = 0f;
        }
    }

    private void rf_Fire()
    {
        float angleStep = (endAngle - startAngle) / bulletsAmmount;
        float angle = startAngle;

        for (int i = 0; i < bulletsAmmount + 1; i++)
        {
            float bulDirX = transform.Find("BulletOutPoint").transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180f);
            float bulDirY = transform.Find("BulletOutPoint").transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180f);

            // passes through calculated directions to instantiate a bullet in those coordinates
            Vector3 bulMoveVector = new Vector3(bulDirX, bulDirY, 0f);
            bulDir = (bulMoveVector - transform.Find("BulletOutPoint").transform.position).normalized;


            GameObject bul = GetComponent<PlayerObjectPool>().rf_GetPlayerBullet();

            if (bul != null)
            {
                bul.transform.position = transform.Find("BulletOutPoint").transform.position;
                bul.transform.rotation = transform.Find("BulletOutPoint").transform.rotation;
                bul.SetActive(true);
                bul.GetComponent<Bullet>().rf_SetMoveDirection(bulDir);
            }
            else { Debug.LogError("Falha ao obter bala!"); }

            angle += angleStep;
        }
    }
    private void rf_FireBulletDestroyer()
    {
        holdingFireTimerSinceLastUse = 0f;


        float bulDirX = transform.Find("BulletOutPoint").transform.position.x;
        float bulDirY = transform.Find("BulletOutPoint").transform.position.y + 1;

        // passes through calculated directions to instantiate a bullet in those coordinates
        Vector3 bulMoveVector = new Vector3(bulDirX, bulDirY, 0f);
        bulDir = (bulMoveVector - transform.Find("BulletOutPoint").transform.position).normalized;



        GameObject bul = GetComponent<PlayerObjectPool>().rf_GetPlayerBullet();
        bul.transform.position = transform.Find("BulletOutPoint").transform.position;
        bul.transform.rotation = transform.rotation;
        bul.SetActive(true);
        bul.GetComponent<Bullet>().rf_SetMoveDirection(bulDir);

    }


    [ServerRpc]
    private void rf_FireServerRPC(Vector3 _bulletOutPos, Quaternion _bulletOutRot)
    {
        float angle = 0;

        for (int i = 0; i < bulletsAmmount + 1; i++)
        {
            float bulDirX = _bulletOutPos.x + Mathf.Sin((angle * Mathf.PI) / 180f);
            float bulDirY = _bulletOutPos.y + Mathf.Cos((angle * Mathf.PI) / 180f);

            // passes through calculated directions to instantiate a bullet in those coordinates
            Vector3 bulMoveVector = new Vector3(bulDirX, bulDirY, 0f);
            bulDir = (bulMoveVector - _bulletOutPos).normalized;

            GameObject bul = Instantiate(bulletPrefab, _bulletOutPos, _bulletOutRot);
            NetworkObject netBul = bul.GetComponent<NetworkObject>();

            if (bul != null)
            {
                if (netBul != null && !netBul.IsSpawned) { netBul.Spawn(true); }

                bul.SetActive(true);
                bul.GetComponent<Bullet>().rf_SetMoveDirection(bulDir);
            }
            else { Debug.LogError("Falha ao obter bala!"); }
        }
    }
    
    [ServerRpc]
    private void rf_FireBulletDestroyerServerRPC(Vector3 _bulletOutPos, Quaternion _bulletOutRot)
    {
        holdingFireTimerSinceLastUse = 0;

        float angle = 0;

        for (int i = 0; i < bulletsAmmount + 1; i++)
        {
            float bulDirX = _bulletOutPos.x + Mathf.Sin((angle * Mathf.PI) / 180f);
            float bulDirY = _bulletOutPos.y + Mathf.Cos((angle * Mathf.PI) / 180f);

            // passes through calculated directions to instantiate a bullet in those coordinates
            Vector3 bulMoveVector = new Vector3(bulDirX, bulDirY, 0f);
            bulDir = (bulMoveVector - _bulletOutPos).normalized;

            GameObject bul = Instantiate(destroyerPrefab, _bulletOutPos, _bulletOutRot);
            NetworkObject netBul = bul.GetComponent<NetworkObject>();

            if (bul != null)
            {
                if (netBul != null && !netBul.IsSpawned) { netBul.Spawn(true); }

                bul.SetActive(true);
            }
            else { Debug.LogError("Falha ao obter bala!"); }
        }
    }
}
