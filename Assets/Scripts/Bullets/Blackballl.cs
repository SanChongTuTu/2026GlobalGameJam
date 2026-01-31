using TMPro;
using UnityEngine;

public class Blackball : MonoBehaviour
{
    [Header("伤害")]
    public float damage = 10f;
    [Header("存在时间")]
    public float lifeTime = 3f;
    [Header("是否可穿透敌人")]
    public bool ifpenetrate = false;
    [Header("是否可穿透墙体")]
    public bool ifpenetratewall = false;
    Rigidbody2D rb;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    void Start() => Destroy(gameObject, lifeTime);

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            Hit(other);

        if (other.CompareTag("Ground")&&!ifpenetratewall)
        {
            Destroy(gameObject);
        }
    }

    void Hit(Collider2D player)
    {
        player.GetComponent<BasicControl>()?.TakeDamage(damage);
        if(ifpenetrate == false)
        Destroy(gameObject); 
    }
}