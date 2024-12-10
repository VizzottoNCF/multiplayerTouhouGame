using Unity.Netcode;
using UnityEngine;

public abstract class BaseEntity : NetworkBehaviour
{
    [Header("Base Entity Stats")]
    [SerializeField] public bool alive = true;
    [SerializeField] public int health;
    [SerializeField] public int maxHealth;
    [SerializeField] public int iFrames = 0;

    private void FixedUpdate()
    {
        // kills entity if 
        if (alive && health <= 0)
        {
            alive = false;
            rf_OnDeath();
        }

        // reduces iframes
        if (iFrames != 0)
        {
            iFrames--;
        }

    }

    public virtual void rf_OnDeath() { }

    public void rf_TakeDamage(int iFramesQuantity = 10)
    {
        // take damage and then give 10 iframes
        if (iFrames <= 0)
        {
            health -= 1;
            iFrames = iFramesQuantity;
        }
    }
}
