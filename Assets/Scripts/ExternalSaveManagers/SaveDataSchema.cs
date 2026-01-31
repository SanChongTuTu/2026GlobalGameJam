using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSaveData
{
    public string sceneName;
    public PlayerSaveData playerData;
    public List<MonsterSaveData> monsterData = new List<MonsterSaveData>();
}

[System.Serializable]
public class PlayerSaveData
{
    public Vector3 position;
    
    public float health;
    public float damage;
    public float moveSpeed;
    public float jumpForce;
    public float attackCooldown;
    public int playerTypeIndex;
    
    public int checkpointIndex;
    
    public int currentMaskIndex;
    public List<int> unlockedMaskIndices = new List<int>(); // Only store indices of unlocked masks
}

[System.Serializable]
public class MonsterSaveData
{
    public string uniqueID;
    public float health;
    public Vector3 position;
    public bool isDead;
}