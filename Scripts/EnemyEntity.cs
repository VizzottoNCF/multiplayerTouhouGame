using Unity.Netcode;
using UnityEngine;

public class EnemyEntity : BaseEntity
{
    private GeneralEnemyAI generalEnemyAI;

    public float changeAITimer = 0f;

    private void Start()
    {
        generalEnemyAI = GetComponent<GeneralEnemyAI>();
    }

    public override void rf_OnDeath() => rf_OnEnemyDeath();

    private void rf_OnEnemyDeath()
    {
        gameObject.GetComponent<NetworkObject>().Despawn();
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!IsOwner) { return; }
        changeAITimer += Time.deltaTime;

        if (changeAITimer > 4f)
        {
            changeAITimer = 0f;
            if (health >= maxHealth / 2)
            {
                int rand = Random.Range(1, 3);
                switch (rand)
                {
                    case 1:
                        generalEnemyAI.rf_OnStateChange(GeneralEnemyAI.EnemyState.Following); 
                        break;
                    case 2:
                        generalEnemyAI.rf_OnStateChange(GeneralEnemyAI.EnemyState.Idling); 
                        break;
                }
            }
            else
            {
                generalEnemyAI.rf_OnStateChange(GeneralEnemyAI.EnemyState.Figure8);
            }
        }
    }
}
