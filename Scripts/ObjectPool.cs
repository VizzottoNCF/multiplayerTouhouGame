using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool bulletPoolInstance;

    [SerializeField] GameObject pooledEnemyBullet;
    [SerializeField] GameObject pooledPlayerBullet;
    public bool notEnoughBulletsInPool = true;

    private List<GameObject> enemyBullets;
    private List<GameObject> playerBullets;

    private void Awake() { bulletPoolInstance = this; }


    private void Start() { enemyBullets = new List<GameObject>(); playerBullets = new List<GameObject>();  }
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
            bul.SetActive(false);
            enemyBullets.Add(bul);
            return bul;
        }

        return null;
    }
    #endregion
    #region Player Bullets
    public GameObject rf_GetPlayerBullet()
    {
        // if we have bullets in our pool, it'll look through the pool to see if there is one not active in hierarchy and return it
        if (playerBullets.Count > 0)
        {
            for (int i = 0; i < playerBullets.Count; i++)
            {
                if (!playerBullets[i].activeInHierarchy)
                {
                    return playerBullets[i];
                }
            }
        }

        // else, will instantiate a new bullet, deactivate it, add it to the pool and then return it to be activated by the emitter
        if (notEnoughBulletsInPool)
        {
            GameObject bul = Instantiate(pooledPlayerBullet);
            bul.SetActive(false);
            playerBullets.Add(bul);
            return bul;
        }

        return null;
    }
    #endregion
}