using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{

    private static GameDataManager instance;
    public static GameDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance=FindObjectOfType<GameDataManager>();
                if (instance == null)
                {
                    Debug.Log("No GameDataManager found!");
                }
            }
            return instance;
        }
    }
    public Transform player;
    public float health;
    public float damage;
    public float moveSpeed;
    public float jumpForce;
    public float attackCooldown;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
