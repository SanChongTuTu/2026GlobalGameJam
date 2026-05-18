using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mask", menuName = "CreateData/Mask", order = 2)]
public class CreateMask : ScriptableObject
{
    [System.Serializable]
    public enum MaskType
    {
        黑铁面具,
        火焰面具,
        疾风面具,
        寒冰面具,
        雷霆面具,
        死亡面具
    }

    [Header("=== 基本信息 ===")]
    [Header("面具名称")]
    public MaskType maskName;

    [Header("面具描述")]
    [TextArea(3, 5)]
    public string description;

    [Header("面具图标")]
    public Sprite icon;

    [Header("面具预制体（用于游戏中的实际表现）")]
    public GameObject maskPrefab;

    [Header("=== 战斗属性 ===")]
    [Header("攻击力加成")]
    public int attackBonus = 0;

    [Header("生命值加成")]
    public int healthBonus = 0;

    [Header("移动速度加成")]
    [Range(0f, 2f)]
    public float speedMultiplier = 1.0f;

    [Header("是否可二段跳")]
    public bool canDoubleJump = false;

    [Header("=== 技能系统 ===")]
    [Header("普通攻击描述")]
    [TextArea(2, 3)]
    public string normalAttackDesc;

    [Header("技能描述")]
    [TextArea(2, 4)]
    public string skillDesc;

    [Header("技能冷却时间")]
    public float skillCooldown = 30f;

    [Header("特殊效果描述")]
    [TextArea(2, 3)]
    public string specialEffectDesc;

    [Header("解锁条件")]
    public string unlockCondition;

    // 从预制体自动读取的信息
    [System.Serializable]
    public class PrefabInfo
    {
        public string prefabName;
        public bool hasAnimator;
        public bool hasCollider;
        public bool hasRigidbody;
        public int componentCount;
        public string assetPath;
    }

    [HideInInspector]
    public PrefabInfo prefabInfo;

    // 自动读取预制体信息的方法
    public void LoadPrefabInfo()
    {
        if (maskPrefab == null)
        {
            Debug.LogWarning($"面具 {maskName} 没有设置预制体");
            return;
        }

        prefabInfo = new PrefabInfo();
        prefabInfo.prefabName = maskPrefab.name;

        // 获取预制体上的组件
        Component[] components = maskPrefab.GetComponents<Component>();
        prefabInfo.componentCount = components.Length;

        // 检查常用组件
        prefabInfo.hasAnimator = maskPrefab.GetComponent<Animator>() != null;
        prefabInfo.hasCollider = maskPrefab.GetComponent<Collider2D>() != null;
        prefabInfo.hasRigidbody = maskPrefab.GetComponent<Rigidbody2D>() != null;

//        // 获取资源路径（编辑器模式下）
//#if UNITY_EDITOR
//        prefabInfo.assetPath = UnityEditor.AssetDatabase.GetAssetPath(maskPrefab);
//#endif

//        Debug.Log($"成功读取面具预制体: {maskPrefab.name}");
    }

    // 获取完整的面具信息（用于图鉴显示）
    public string GetFullInfo()
    {
        string info = $"<b>{maskName}</b>\n\n";
        info += $"<color=#FFA500>描述:</color> {description}\n\n";
        info += $"<color=#FFA500>普通攻击:</color> {normalAttackDesc}\n";
        info += $"<color=#FFA500>攻击力加成:</color> +{attackBonus}\n";

        if (healthBonus > 0)
            info += $"<color=#FFA500>生命值加成:</color> +{healthBonus}\n";

        if (speedMultiplier != 1.0f)
            info += $"<color=#FFA500>速度加成:</color> ×{speedMultiplier:F1}\n";

        if (canDoubleJump)
            info += $"<color=#FFA500>特殊能力:</color> 可二段跳\n";

        info += $"\n<color=#FFA500>技能:</color> {skillDesc}\n";
        info += $"<color=#FFA500>冷却时间:</color> {skillCooldown}秒\n\n";

        if (!string.IsNullOrEmpty(specialEffectDesc))
        {
            info += $"<color=#FFA500>特殊效果:</color> {specialEffectDesc}\n\n";
        }

        if (!string.IsNullOrEmpty(unlockCondition))
        {
            info += $"<color=#00FF00>解锁条件:</color> {unlockCondition}";
        }

        return info;
    }

    // 获取简化的游戏内信息
    public string GetShortInfo()
    {
        return $"{maskName}: +{attackBonus}攻击 | {normalAttackDesc}";
    }
}