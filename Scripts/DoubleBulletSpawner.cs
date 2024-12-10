using UnityEngine;
using Unity.Netcode;
using System.Globalization;

public class DoubleBulletSpawner : NetworkBehaviour
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
    [SerializeField][Range(-1f, 1f)] float firingDirection;
    [SerializeField] private float firingRate = 1f;
    private float timerFire = 0f;

    private void Update()
    {
        // timer to continues shooting rounds.
        // using this instead of InvokeRepeating to be able to change bulletfire speed
        timerFire += Time.deltaTime;
        if (timerFire >= firingRate) { rf_Fire(); timerFire = 0f; }

        
        // if spawner is a spin type, uses euler angle to spin around z axis
        if (spawnerType == DoubleBulletSpawner.SpawnerType.Spin) { transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z + firingDirection); }

    }

    private void rf_Fire()
    {
        float angleStep = (endAngle - startAngle) / bulletsAmmount;
        float angle = startAngle;

        for (int i = 0; i <= 1 + 1; i++)
        {
            float bulDirX = transform.position.x + Mathf.Sin(((angle + 180f * i) * Mathf.PI) / 180f);
            float bulDirY = transform.position.y + Mathf.Cos(((angle + 180f * i) * Mathf.PI) / 180f);

            // passes through calculated directions to instantiate a bullet in those coordinates
            Vector3 bulMoveVector = new Vector3(bulDirX, bulDirY, 0f);
            bulDir = (bulMoveVector - transform.position).normalized;


            GameObject bul = ObjectPool.bulletPoolInstance.rf_GetEnemyBullet();
                //bul.speed = 
                bul.transform.position = transform.position;
                bul.transform.rotation = transform.rotation;
                bul.SetActive(true);
                bul.GetComponent<Bullet>().rf_SetMoveDirection(bulDir);
        }

        angle += 10f;
        if (angle >= 360) { angle = 0f; }
    }
}
