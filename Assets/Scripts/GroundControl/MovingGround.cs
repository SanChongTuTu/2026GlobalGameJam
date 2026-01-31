using System.Collections;
using UnityEngine;

public class MovingGround : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("The world position where the platform should move to.")]
    public Vector3 targetPosition;
    public float speed = 3f;
    public float waitTime = 2f;

    private Vector3 _startPos;
    private bool _isSequenceActive = false;

    void Start()
    {
        _startPos = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !_isSequenceActive)
        {
            StartCoroutine(MovementSequence());
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }

    private IEnumerator MovementSequence()
    {
        _isSequenceActive = true;
        
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;
        
        yield return new WaitForSeconds(waitTime);
        
        while (Vector3.Distance(transform.position, _startPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, _startPos, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = _startPos;
        
        _isSequenceActive = false;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 start = Application.isPlaying ? _startPos : transform.position;
        Gizmos.DrawLine(start, targetPosition);
        Gizmos.DrawWireCube(targetPosition, transform.localScale);
    }
}