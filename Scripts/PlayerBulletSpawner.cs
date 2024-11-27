using Unity.VisualScripting;
using UnityEngine;

public class PlayerBulletSpawner : MonoBehaviour
{
    public enum SpawnerType { Straight, Spin }

    [Header("Bullet Attributes")]
    /// <summary> ammount to be fired at once </summary>
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
    private float holdingFireTimerSinceLastUse = 0f;

    private void Start() { Invoke("rf_Fire", 0f); }

    private void Update()
    {
        // timer to continues shooting rounds.
        // using this instead of InvokeRepeating to be able to change bullet fire speed
        timerFire += Time.deltaTime;
        holdingFireTimerSinceLastUse += Time.deltaTime;

        //if (timerFire >= firingRate && Input.GetKey(KeyCode.Space)) { rf_Fire(); timerFire = 0f; }
        if (timerFire >= firingRate && !holdingFire) { rf_Fire(); timerFire = 0f; }

        // hold your constant fire to send forth a big projectile killing bullet
        holdingFire = Input.GetKey(KeyCode.Space);
        if (holdingFire)
        {
            holdingFireTimer += Time.deltaTime;
        }

        // when fire is held for 2 seconds, once it's sustained again, release bullet destroyer
        if (Input.GetKeyUp(KeyCode.Space) && holdingFireTimer >= 2f && holdingFireTimerSinceLastUse >= 15f)
        {
            rf_FireBulletDestroyer();
        }

    }

    private void rf_Fire()
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


            GameObject bul = ObjectPool.bulletPoolInstance.rf_GetPlayerBullet();
            bul.transform.position = transform.position;
            bul.transform.rotation = transform.rotation;
            bul.SetActive(true);
            bul.GetComponent<Bullet>().rf_SetMoveDirection(bulDir);

            angle += angleStep;
        }
    }
    private void rf_FireBulletDestroyer()
    {
        holdingFireTimerSinceLastUse = 0f;
         

        float bulDirX = transform.position.x;
        float bulDirY = transform.position.y + 1;

        // passes through calculated directions to instantiate a bullet in those coordinates
        Vector3 bulMoveVector = new Vector3(bulDirX, bulDirY, 0f);
        bulDir = (bulMoveVector - transform.position).normalized;


        GameObject bul = ObjectPool.bulletPoolInstance.rf_GetPlayerBullet();
        bul.transform.position = transform.position;
        bul.transform.rotation = transform.rotation;
        bul.SetActive(true);
        bul.GetComponent<Bullet>().rf_SetMoveDirection(bulDir);

    }
}
