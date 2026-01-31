using UnityEngine;

public class Elevator : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("How high the elevator goes relative to its starting position")]
    public float heightCap = 5f;
    [Tooltip("Movement speed")]
    public float speed = 3f;

    private Vector3 _startPos;
    private Vector3 _targetPos;
    private bool _isPlayerOn;

    void Start()
    {
        _startPos = transform.position;
        _targetPos = _startPos + new Vector3(0, heightCap, 0);
    }

    void FixedUpdate()
    {
        Vector3 destination = _isPlayerOn ? _targetPos : _startPos;
        
        if (transform.position != destination)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _isPlayerOn = true;
            
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _isPlayerOn = false;
            
            collision.transform.SetParent(null);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 start = Application.isPlaying ? _startPos : transform.position;
        Vector3 end = start + new Vector3(0, heightCap, 0);
        
        Gizmos.DrawLine(start, end);
        Gizmos.DrawWireCube(end, transform.localScale);
    }
}