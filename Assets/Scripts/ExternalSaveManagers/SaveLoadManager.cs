using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance;

    private string saveFileName = "gamedata.json";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void SaveGame()
    {
        GameSaveData data = new GameSaveData();
        
        data.sceneName = SceneManager.GetActiveScene().name;
        
        data.playerData = GatherPlayerData();
        
        data.monsterData = GatherMonsterData();
        
        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(Application.persistentDataPath, saveFileName);
        File.WriteAllText(path, json);

        Debug.Log($"Game Saved to: {path}");
    }
    
    public void LoadGame()
    {
        string path = Path.Combine(Application.persistentDataPath, saveFileName);
        if (!File.Exists(path))
        {
            Debug.LogWarning("No save file found!");
            return;
        }

        string json = File.ReadAllText(path);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);
        
        StartCoroutine(RestoreGameRoutine(data));
    }

    private IEnumerator RestoreGameRoutine(GameSaveData data)
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene != data.sceneName)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(data.sceneName);
            while (!asyncLoad.isDone) yield return null;
        }
        
        yield return new WaitForEndOfFrame();
        
        ApplyPlayerData(data.playerData);
        
        ApplyMonsterData(data.monsterData);

        Debug.Log("Game Loaded Successfully!");
    }

    private PlayerSaveData GatherPlayerData()
    {
        PlayerSaveData pd = new PlayerSaveData();
        GameDataManager gdm = GameDataManager.Instance;
        
        pd.position = gdm.player != null ? gdm.player.position : gdm.lastPlayerPosition;
        pd.health = gdm.health;
        pd.damage = gdm.damage;
        pd.moveSpeed = gdm.moveSpeed;
        pd.jumpForce = gdm.jumpForce;
        pd.attackCooldown = gdm.attackCooldown;
        pd.playerTypeIndex = (int)gdm.playerType;
        
        var maskManager = FindObjectOfType<PlayerMaskManager>();
        if (maskManager != null)
        {
            pd.currentMaskIndex = maskManager.GetCurrentMaskIndex();
            pd.unlockedMaskIndices = maskManager.GetUnlockedIndices();
        }
        
        var cpManager = FindObjectOfType<CheckpointManager>();
        if (cpManager != null)
        {
            pd.checkpointIndex = cpManager.GetCurrentCheckpointIndex();
        }

        return pd;
    }

    private List<MonsterSaveData> GatherMonsterData()
    {
        List<MonsterSaveData> list = new List<MonsterSaveData>();
        Monster[] monsters = FindObjectsOfType<Monster>();

        foreach (var m in monsters)
        {
            SaveID idComp = m.GetComponent<SaveID>();
            if (idComp != null)
            {
                MonsterSaveData md = new MonsterSaveData();
                md.uniqueID = idComp.uniqueID;
                md.health = m.health;
                md.position = m.transform.position;
                md.isDead = m.IsDead();
                list.Add(md);
            }
        }
        return list;
    }

    private void ApplyPlayerData(PlayerSaveData pd)
    {
        GameDataManager gdm = GameDataManager.Instance;
        
        gdm.health = pd.health;
        gdm.damage = pd.damage;
        gdm.moveSpeed = pd.moveSpeed;
        gdm.jumpForce = pd.jumpForce;
        gdm.attackCooldown = pd.attackCooldown;
        gdm.playerType = (GameDataManager.Type)pd.playerTypeIndex;
        
        if (gdm.player != null)
        {
            gdm.player.position = pd.position;
            Rigidbody rb = gdm.player.GetComponent<Rigidbody>();
            if (rb != null) rb.velocity = Vector3.zero;
        }
        gdm.lastPlayerPosition = pd.position;
        
        var cpManager = FindObjectOfType<CheckpointManager>();
        if (cpManager != null) cpManager.SetCurrentCheckpointIndex(pd.checkpointIndex);
        
        var maskManager = FindObjectOfType<PlayerMaskManager>();
        if (maskManager != null)
        {
            maskManager.LoadUnlockedIndices(pd.unlockedMaskIndices);
            maskManager.EquipMask(pd.currentMaskIndex);
        }
    }

    private void ApplyMonsterData(List<MonsterSaveData> mDataList)
    {
        Dictionary<string, MonsterSaveData> dataMap = new Dictionary<string, MonsterSaveData>();
        foreach (var md in mDataList)
        {
            if (!dataMap.ContainsKey(md.uniqueID)) dataMap.Add(md.uniqueID, md);
        }
        
        Monster[] sceneMonsters = FindObjectsOfType<Monster>();
        foreach (var m in sceneMonsters)
        {
            SaveID idComp = m.GetComponent<SaveID>();
            if (idComp != null && dataMap.ContainsKey(idComp.uniqueID))
            {
                MonsterSaveData data = dataMap[idComp.uniqueID];

                if (data.isDead)
                {
                    m.gameObject.SetActive(false);
                }
                else
                {
                    m.health = data.health;
                    m.transform.position = data.position;
                }
            }
        }
    }
}