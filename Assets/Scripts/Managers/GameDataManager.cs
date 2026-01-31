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
    public enum Type
    {
        none,
        iron,
        fire,
        wind,
        ice,
        thunder,
        death
    }
    [Header("玩家")]
    public Transform player;
    [Header("玩家当前面具")]
    public Type playerType;
    [Header("玩家生命值")]
    public float health;
    [Header("玩家攻击力")]
    public float damage;
    [Header("玩家移速")]
    public float moveSpeed;
    [Header("玩家跳跃高度")]
    public float jumpForce;
    [Header("玩家攻击间隔")]
    public float attackCooldown;

    float savespeed;//记录当前速度
    float savedamage;
    float saveattackcooldown;

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
        savespeed=moveSpeed;
        savedamage = damage;
        saveattackcooldown = attackCooldown;
    }


    void Update()
    {
        if (playerType != Type.ice && player.GetComponent<LineRenderer>() != null)
        {
            player.GetComponent<LineRenderer>().positionCount = 0;
            Destroy(player.GetComponent<LineRenderer>());
        }

        if(playerType == Type.wind)
        {
            WindMask wind = player.GetComponentInChildren<WindMask>();
            bool strength = wind.strength;
            if (strength)
            {
                moveSpeed = savespeed * 2f;
                damage = savedamage*1.5f;
                attackCooldown = saveattackcooldown * 0.5f;
            }
            else
            {
                moveSpeed = savespeed * 1.2f;
                damage = savedamage;
                attackCooldown = saveattackcooldown;
            }
            player.GetComponent<JumpController>().jumptime = 2;
        }
        else
        {
            moveSpeed = savespeed;
            damage=savedamage;
        }
    }
}
