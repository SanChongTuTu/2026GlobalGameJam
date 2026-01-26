using UnityEngine;

public class Monster : MonoBehaviour
{
    public enum FactionType { Red, Blue }

    [Header("Monster Identity")]
    public FactionType faction;
    
    [Header("Stats")]
    public float health = 100f;
    public float damage = 10f;

    private bool _isDead = false;
    
    private void OnValidate()
    {
        UpdateVisuals();
    }

    private void Start()
    {
        UpdateVisuals();
    }
    
    public void TakeDamage(float amount)
    {
        if (_isDead) return;

        health -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Health: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        _isDead = true;
        Debug.Log($"{gameObject.name} has been defeated!");
        
        Destroy(gameObject); 
    }

    private void UpdateVisuals()
    {
        Renderer renderer = GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            renderer.sharedMaterial.color = (faction == FactionType.Red) ? Color.red : Color.blue;
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Monster hit the player!");
        }
    }
}
