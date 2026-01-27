using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    [Header("Checkpoint Setup")]
    public List<Transform> checkpoints = new List<Transform>();
    
    private int _currentCheckpointIndex = -1;
    
    private Vector3 _initialStartPosition;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        if (player != null)
        {
            _initialStartPosition = player.transform.position;
        }
        else
        {
            Debug.LogError("CheckpointManager could not find Player to record start position!");
        }
    }

    public void UnlockCheckpoint(Transform checkpointTransform)
    {
        int index = checkpoints.IndexOf(checkpointTransform);

        if (index == -1) return;
        
        if (index > _currentCheckpointIndex)
        {
            _currentCheckpointIndex = index;
            Debug.Log($"Checkpoint {_currentCheckpointIndex} Unlocked!");
        }
    }

    public void RespawnPlayer(GameObject player)
    {
        Vector3 targetPos;

        if (_currentCheckpointIndex == -1)
        {
            targetPos = _initialStartPosition;
            Debug.Log("Respawning at Start Position...");
        }
        else
        {
            targetPos = checkpoints[_currentCheckpointIndex].position;
            Debug.Log($"Respawning at Checkpoint {_currentCheckpointIndex}...");
        }
        
        player.transform.position = targetPos;
        
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
    
    public void ResetProgress()
    {
        _currentCheckpointIndex = -1;
    }
}