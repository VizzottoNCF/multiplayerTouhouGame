using Unity.Netcode;
using UnityEngine;

public class GeneralEnemyAI : NetworkBehaviour
{
    public enum EnemyState { Idling, Figure8, Following }

    public EnemyState enemyState;

    [SerializeField] private Transform EnemyPoint1;
    [SerializeField] private Transform EnemyPoint2;
    [SerializeField] private Transform EnemyPoint3;
    [SerializeField] private Transform[] enemyPointAmount;

    [SerializeField] private BulletSpawner spawner1; // Idle emitter
    [SerializeField] private BulletSpawner spawner2; // Figure 8 emitter
    [SerializeField] private BulletSpawner spawner3; // Following emitter 1
    [SerializeField] private BulletSpawner spawner4; // Following emitter 2

    private bool reachedPoint = false;
    private bool ranSetup = false;
    [SerializeField] float speed = 5f;
    private GameObject targetPlayer;
    private Transform targetTransform;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        // Finds own emitters and disables them
        spawner1 = transform.Find("Emitter1")?.GetComponent<BulletSpawner>();
        spawner2 = transform.Find("Emitter2")?.GetComponent<BulletSpawner>();
        spawner3 = transform.Find("Emitter3")?.GetComponent<BulletSpawner>();
        spawner4 = transform.Find("Emitter4")?.GetComponent<BulletSpawner>();

        spawner1.enabled = false;
        spawner2.enabled = false;
        spawner3.enabled = false;
        spawner4.enabled = false;

        // finds points for idleAI and bundles in array
        if (EnemyPoint1 == null) { EnemyPoint1 = GameObject.Find("EnemyPoint1").transform; }
        if (EnemyPoint2 == null) { EnemyPoint2 = GameObject.Find("EnemyPoint2").transform; }
        if (EnemyPoint3 == null) { EnemyPoint3 = GameObject.Find("EnemyPoint3").transform; }

        enemyPointAmount = new Transform[] { EnemyPoint1, EnemyPoint2, EnemyPoint3 };
    }

    private void Update()
    {
        switch (enemyState)
        {
            case EnemyState.Idling:
                rf_IdleAI();
                break;

            case EnemyState.Figure8:
                rf_Figure8AI();
                break;

            case EnemyState.Following:
                rf_FollowingAI();
                break;
        }
    }

    private void rf_IdleAI()
    {
        if (!ranSetup)
        {
            targetTransform = enemyPointAmount[Random.Range(0, enemyPointAmount.Length)];
            Debug.Log(targetTransform);
            ranSetup = true;

            spawner1.enabled = false;
            spawner2.enabled = false;
            spawner3.enabled = false;
            spawner4.enabled = false;
        }

        // moves towards point and enables spin blasters

        Vector2 currentPos = transform.position;
        Vector2 targetPos = targetTransform.position;

        if (Vector2.Distance(currentPos, targetPos) > 0.1f && !reachedPoint) 
        {
            transform.position = Vector2.MoveTowards(currentPos, targetPos, speed * Time.deltaTime);
            reachedPoint = true;
        }
        else { spawner1.enabled = true; }
    }

    private void rf_Figure8AI()
    {
        // goes to center and then figure 8
        if (!ranSetup)
        {
            targetTransform = EnemyPoint3;

            spawner1.enabled = false;
            spawner2.enabled = false;
            spawner3.enabled = false;
            spawner4.enabled = false;

            ranSetup = true;
        }

        // moves towards point and enables figure8

        Vector2 currentPos = transform.position;
        Vector2 targetPos = targetTransform.position;
        if (Vector2.Distance(currentPos, targetPos) > 0.1f && !reachedPoint)
        {
            //Debug.Log(Vector2.Distance(currentPos, targetPos));
            transform.position = Vector2.MoveTowards(currentPos, targetPos, speed * Time.deltaTime);
        }
        else 
        {
            reachedPoint = true;
            spawner2.enabled = true;
            _animator.SetBool("Figure8", true); 
        }
    }

    private void rf_FollowingAI()
    {
        // target setup
        if (!ranSetup)
        {
            GameObject[] targetAmount = GameObject.FindGameObjectsWithTag("Player");

            if (targetAmount.Length > 0)
            {
                targetPlayer = targetAmount[Random.Range(0, targetAmount.Length)];
                ranSetup = true;
            }
            else { Debug.LogWarning("No objects with the 'Player' tag were found."); }

            spawner1.enabled = false;
            spawner2.enabled = false;
            spawner3.enabled = true;
            spawner4.enabled = true;
        }

        // gather target position and follow it gradually staying slightly above him
        Vector2 targetPos = new Vector2(targetPlayer.transform.position.x, targetPlayer.transform.position.y + 9);
        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    }

    public void rf_OnStateChange(EnemyState state)
    {
        // deactivates initial setup for targets and changes enemy state
        ranSetup = false;
        reachedPoint = false;
        enemyState = state;
        _animator.SetBool("Figure8", false);
    }
}