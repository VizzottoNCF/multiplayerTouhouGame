using Unity.VisualScripting;
using UnityEditor.Rendering.Analytics;
using UnityEngine;

public class BulletDestroyer : MonoBehaviour
{
    private CircleCollider2D CircleCollider2D;
    private Vector2 spawnpoint;
    [SerializeField]
    float timer = 0f;

    [Header("Attributes")]
    [SerializeField] float life = 4f;
    [SerializeField] float speed = 5f;
    [SerializeField] float maxDistance = 4f;


    private void Start()
    {
        spawnpoint = new Vector2(transform.position.x, transform.position.y);

        CircleCollider2D = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        // kills bullet if time since spawns exceeds life
        if (timer > life) { Destroy(gameObject); }

        if (timer > 2f) { speed -= Time.deltaTime; }

        timer += Time.deltaTime;

        // calls movement function
        transform.position = rf_Movement(timer);

    }

    private Vector2 rf_Movement(float timer)
    {
        // moves bullet ahead as by it's rotation
        float y = timer * speed * transform.up.y;
        return new Vector2(spawnpoint.x, y + spawnpoint.y); // returns new position
    }

    private void OnTriggerEnter(Collider other)
    {
        // if collides with bullets, kills bullets
        if (other.gameObject.tag == "Bullet") { other.gameObject.SetActive(false); }
    }
}
