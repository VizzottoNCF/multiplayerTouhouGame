using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerObjectPool : NetworkBehaviour
{
    [SerializeField] GameObject pooledPlayerBullet;
    public bool notEnoughBulletsInPool = true;
    private List<GameObject> playerBullets;

    private void Start() { playerBullets = new List<GameObject>(); }

    #region Player Bullets

    //public GameObject rf_GetPlayerBullet()
    //{
    //    // if we have bullets in our pool, it'll look through the pool to see if there is one not active in hierarchy and return it
    //    if (playerBullets.Count > 0)
    //    {
    //        for (int i = 0; i < playerBullets.Count; i++)
    //        {
    //            if (!playerBullets[i].activeInHierarchy)
    //            {
    //                return playerBullets[i];
    //            }
    //        }
    //    }

    //    // else, will instantiate a new bullet, deactivate it, add it to the pool and then return it to be activated by the emitter
    //    if (notEnoughBulletsInPool)
    //    {
    //        // call server rpc to instantiate gameObject/spawn networkObject and insert it into local pool, then will search through for loop again to return a bullet
    //        rf_RequestBulletFromServerServerRpc();

    //        for (int i = 0; i < playerBullets.Count; i++)
    //        {
    //            if (!playerBullets[i].activeInHierarchy)
    //            {
    //                Debug.Log("Retornando " + playerBullets[i]);
    //                return playerBullets[i];
    //            }
    //        }
    //    }

    //    return null;
    //}

    public GameObject rf_GetPlayerBullet()
    {
        if (playerBullets.Count > 0)
        {
            foreach (var bullet in playerBullets)
            {
                if (bullet != null && !bullet.activeInHierarchy)
                {
                    return bullet;
                }
            }
        }

        if (playerBullets.Count > 15) { notEnoughBulletsInPool = false; }

        if (notEnoughBulletsInPool)
        {
            rf_RequestBulletFromServerServerRpc();
        }

        foreach (var bullet in playerBullets)
        {
            if (bullet != null && !bullet.activeInHierarchy)
            {
                return bullet;
            }
        }

        Debug.LogError("Nenhuma bala disponivel");
        return null;
    }
    #endregion

    #region Server-Client Relationship

    [ServerRpc(RequireOwnership = true)]
    private void rf_RequestBulletFromServerServerRpc(ServerRpcParams rpcParams = default)
    {
        GameObject bul = Instantiate(pooledPlayerBullet);
        var networkObject = bul.GetComponent<NetworkObject>();

        if (networkObject != null)
        {
            if (!networkObject.IsSpawned) { networkObject.Spawn(true); }
            bul.SetActive(false); 
            playerBullets.Add(bul);

            rf_SendBulletToClientClientRpc(networkObject.NetworkObjectId, rpcParams.Receive.SenderClientId);
        }
        else { Debug.LogError("NetworkObject não encontrado ao instanciar a bala!"); }
    }

    [ClientRpc]
    private void rf_SendBulletToClientClientRpc(ulong bulletNetworkObject, ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(bulletNetworkObject, out var networkObject))
            {
                GameObject bullet = networkObject.gameObject;
                if (bullet != null && !playerBullets.Contains(bullet))
                {
                    playerBullets.Add(bullet);
                    Debug.Log("Bala adicionada ao cliente.");
                }
            }
            else
            {
                Debug.LogError("NetworkObject não encontrado no SpawnManager!");
            }
        }
    }


    //[ClientRpc]
    //private void rf_SendBulletToClientClientRpc(ulong bulletNetworkObject, ulong clientId)
    //{

    //    if (NetworkManager.Singleton.LocalClientId == clientId)
    //    {
    //        Debug.Log("CLIENT RPC1");
    //        NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[bulletNetworkObject];
    //        if (networkObject != null)
    //        {
    //            GameObject bullet = networkObject.gameObject;
    //            playerBullets.Add(bullet);
    //            Debug.Log("CLIENT RPC ADDED BULLET");
    //        }
    //    }
    //}

    #endregion
}