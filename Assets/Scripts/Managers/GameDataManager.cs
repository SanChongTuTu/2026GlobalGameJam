using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    
    [Header("Persistence")]
    public Vector3 lastPlayerPosition;
    public string currentSceneName;
    
    [Header("���")]
    public Transform player;
    [Header("��ҵ�ǰ���")]
    public Type playerType;
    [Header("�������ֵ")]
    public float health;
    [Header("��ҹ�����")]
    public float damage;
    [Header("�������")]
    public float moveSpeed;
    [Header("�����Ծ�߶�")]
    public float jumpForce;
    [Header("��ҹ������")]
    public float attackCooldown;

    public bool banE = false;
    public bool banL = false;

    float savespeed;//��¼��ǰ�ٶ�
    float savedamage;
    float saveattackcooldown;

    private void Awake()
    {
        //if (instance == null)
        //{
        //    instance = this;
        //    DontDestroyOnLoad(gameObject);
        //}
        //else if (instance != this)
        //{
        //    Destroy(gameObject);
        //}
    }


    void Start()
    {
        savespeed=moveSpeed;
        savedamage = damage;
        saveattackcooldown = attackCooldown;
    }


    void Update()
    {
        if (player != null)
        {
            lastPlayerPosition = player.position;
        }
        
        string activeScene = SceneManager.GetActiveScene().name;
        if (currentSceneName != activeScene)
        {
            currentSceneName = activeScene;
            Debug.Log($"Scene changed to: {currentSceneName}");
        }
        
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
                moveSpeed = savespeed * 1.8f;
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
    
    public void SavePlayerData()
    {
        if (player != null)
        {
            lastPlayerPosition = player.position;
        }
        currentSceneName = SceneManager.GetActiveScene().name;
        
        PlayerPrefs.SetFloat("PlayerX", lastPlayerPosition.x);
        PlayerPrefs.SetFloat("PlayerY", lastPlayerPosition.y);
        PlayerPrefs.SetString("SavedScene", currentSceneName);
        PlayerPrefs.Save();
    }

    public void ChangeMask(int id)
    {
        playerType = (Type)id;
        PlayerInfoManager.Instance.SkillCoolDown(10,10);
    }
}
