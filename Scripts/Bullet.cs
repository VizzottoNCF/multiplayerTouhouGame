using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector2 moveDirection;
    [SerializeField] public float speed = 1f;


    private void Start()
    {
        //speed = 5f;
    }

    private void OnEnable()
    {
        Invoke("rf_Destroy", 3f);
    }

    /// <summary> Advances bullet </summary>
    private void FixedUpdate() { this.transform.Translate(moveDirection * speed * Time.deltaTime); }

    /// <summary> Gets direction set by emitter </summary>
    public void rf_SetMoveDirection(Vector2 dir) { moveDirection = dir; }


    private void rf_Destroy() { gameObject.SetActive(false); }

    private void OnDisable() { CancelInvoke(); }

    /// <summary> Take damage when hit by bullet </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // check if either player bullet or enemy bullet, then check for target, reduce hp and give iframes
        // rf_TakeDamage() param == iframe quantity
        if (gameObject.CompareTag("PlayerBullet"))
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                collision.GetComponent<EnemyEntity>().rf_TakeDamage(10);

                // deactivate bullet if it damages
                gameObject.SetActive(false);
            }
        }
        
        if (gameObject.CompareTag("Bullet"))
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                collision.GetComponent<EnemyEntity>().rf_TakeDamage(15);
            }
        }
    }
}
