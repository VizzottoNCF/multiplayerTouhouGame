using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ObjectPool : NetworkBehaviour
{
    [SerializeField] GameObject[] Walls;
    public static ObjectPool bulletPoolInstance;

    [SerializeField] GameObject pooledEnemyBullet;
    public bool notEnoughBulletsInPool = true;

    private List<GameObject> enemyBullets;
    private void Awake() { bulletPoolInstance = this; }
    private void Start() { enemyBullets = new List<GameObject>(); }

    #region Enemy Bullets
    public GameObject rf_GetEnemyBullet()
    {
        // if we have bullets in our pool, it'll look through the pool to see if there is one not active in hierarchy and return it
        if (enemyBullets.Count > 0)
        {
            for (int i = 0; i < enemyBullets.Count; i++)
            {
                if (!enemyBullets[i].activeInHierarchy)
                {
                    return enemyBullets[i];
                }
            }
        }

        // else, will instantiate a new bullet, deactivate it, add it to the pool and then return it to be activated by the emitter
        if (notEnoughBulletsInPool)
        {
            GameObject bul = Instantiate(pooledEnemyBullet);
            var networkObject = bul.GetComponent<NetworkObject>();
            if (networkObject != null) { networkObject.Spawn(true); } else { Debug.LogWarning("NETWORK OBJECT NULL"); }
            bul.SetActive(false);
            enemyBullets.Add(bul);
            return bul;
        }

        return null;
    }
    #endregion

    #region Remove from Pool
    public void rf_RemoveBulletFromPool()
    {

    }
    #endregion
}