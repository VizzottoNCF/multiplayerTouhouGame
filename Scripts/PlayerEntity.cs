using Unity.Netcode;
using UnityEngine;

public class PlayerEntity : BaseEntity
{
    [SerializeField] GameObject hp;
    private PlayerHealthPool playerHealth;
    private void Start()
    {
        // each player adds their total health to a pool shared among all
        playerHealth = hp.transform.Find("HP").gameObject.GetComponent<PlayerHealthPool>();

        Debug.Log(playerHealth);

        playerHealth.totalHealth += maxHealth;
        playerHealth.currentHealth += maxHealth;
    }

    public void rf_TakeDamage(int iFramesQuantity = 10)
    {
        // take damage and then give 10 iframes
        if (iFrames <= 0)
        {
            //health -= 1;
            iFrames = iFramesQuantity;

            // deal damage via server rpc to player health pool
            rf_SendDamageToCanvasServerRPC();
        }
    }

    [ServerRpc]
    private void rf_SendDamageToCanvasServerRPC()
    {
        playerHealth.currentHealth--;
        playerHealth.rf_SetValue();// ClientRPC();
    }
}
