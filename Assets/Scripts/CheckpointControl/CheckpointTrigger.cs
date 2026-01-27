using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    private CheckpointManager _manager;

    void Start()
    {
        _manager = FindObjectOfType<CheckpointManager>();
        
        if (_manager == null)
        {
            Debug.LogError("No CheckpointManager found in the scene!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _manager.UnlockCheckpoint(transform);
        }
    }
}