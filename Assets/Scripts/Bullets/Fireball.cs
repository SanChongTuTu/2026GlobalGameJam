using UnityEngine;

public class Fireball : MonoBehaviour
{
    [Header("ÃüÖÐ²ÎÊý")]
    public float damage = 10f;
    public float lifeTime = 3f;

    Rigidbody2D rb;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    void Start() => Destroy(gameObject, lifeTime);

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
            Hit(other);

        if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }

    void Hit(Collider2D enemy)
    {
        enemy.GetComponent<Monster>()?.TakeDamage(damage);
        Destroy(gameObject); 
    }
}