using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MonsterController : MonoBehaviour
{
    [Header("面板引用")]
    public GameObject monsterPanel;       // 怪物图鉴列表面板
    public GameObject monsterDetailPanel; // 怪物详情面板

    [Header("7个怪物按钮")]
    public Button[] monsterButtons = new Button[7];

    [Header("详情页按钮")]
    public Button closeDetailButton;      // 详情页关闭按钮（回主页）
    public Button backDetailButton;       // 详情页返回按钮（回列表）

    [Header("怪物面板按钮")]
    public Button backToGalleryButton;    // 返回图鉴主面板按钮
    public Button closeMonsterPanelButton;// 关闭怪物图鉴按钮（回主页）

    [Header("详情页UI组件")]
    public Image monsterIcon;             // 怪物图片
    public Text monsterName;              // 怪物名称
    public Text monsterHP;                // 生命值
    public Text monsterAttack;            // 攻击力
    public Text monsterMoveSpeed;         // 移动速度
    public Text monsterAttackType;        // 攻击方式
    public Text monsterSpecial;           // 特殊技能/特性

    // 当前选择的怪物索引
    private int currentMonsterIndex = -1;

    // 怪物数据（根据策划案硬编码，或者从Manager获取）
    private readonly string[] monsterNames = new string[]
    {
        "短刀骷髅", "甲壳虫", "蛇", "法杖骷髅",
        "骷髅骑士", "吸血鬼", "黑暗面具人"
    };

    private readonly string[] monsterDescs = new string[]
    {
        "生命值：低\n攻击力：低\n移动速度：中\n攻击方式：近战\n\n最基础的敌人，手持短刀进行近战攻击。",
        "生命值：中\n攻击力：低\n移动速度：中\n攻击方式：近战\n\n拥有坚硬甲壳的昆虫，防御力较高。",
        "生命值：很低\n攻击力：中\n移动速度：快\n攻击方式：近战\n\n行动敏捷的毒蛇，死后会在原地留下持续10秒的毒液，踩到会受到伤害。",
        "生命值：低\n攻击力：低\n移动速度：中\n攻击方式：远程\n\n使用法杖进行远程法术攻击的骷髅法师。",
        "生命值：高\n攻击力：高\n移动速度：中/快\n攻击方式：近战\n\n精英怪物。生命值降至50%以下时会进入狂暴状态，移动速度大幅提升。",
        "生命值：中\n攻击力：中\n移动速度：中\n攻击方式：近战/远程\n\n飞行单位。近战攻击命中时会吸取玩家生命值回复自身。",
        "生命值：高\n攻击力：高\n移动速度：快\n攻击方式：近战/远程\n\n最终BOSS。生命值降至50%后攻击力增强，攻击速度加快，并释放一次全图范围攻击。"
    };

    void Start()
    {
        if (monsterDetailPanel != null)
            monsterDetailPanel.SetActive(false);

        SetupButtonEvents();
        Debug.Log("MonsterController初始化完成");
    }

    void SetupButtonEvents()
    {
        // 1. 7个怪物按钮
        for (int i = 0; i < monsterButtons.Length; i++)
        {
            if (monsterButtons[i] == null) continue;

            int index = i;
            monsterButtons[i].onClick.RemoveAllListeners();
            monsterButtons[i].onClick.AddListener(() =>
            {
                OnMonsterButtonClicked(index);
            });
        }

        // 2. 详情页关闭按钮（回主页）
        if (closeDetailButton != null)
        {
            closeDetailButton.onClick.RemoveAllListeners();
            closeDetailButton.onClick.AddListener(() =>
            {
                OnCloseDetailClicked();
            });
        }

        // 3. 详情页返回按钮（回列表）
        if (backDetailButton != null)
        {
            backDetailButton.onClick.RemoveAllListeners();
            backDetailButton.onClick.AddListener(() =>
            {
                OnBackDetailClicked();
            });
        }

        // 4. 怪物面板返回按钮（回图鉴主面板）
        if (backToGalleryButton != null)
        {
            backToGalleryButton.onClick.RemoveAllListeners();
            backToGalleryButton.onClick.AddListener(() =>
            {
                OnBackToGalleryClicked();
            });
        }

        // 5. 怪物面板关闭按钮（回主页）
        if (closeMonsterPanelButton != null)
        {
            closeMonsterPanelButton.onClick.RemoveAllListeners();
            closeMonsterPanelButton.onClick.AddListener(() =>
            {
                OnCloseMonsterPanelClicked();
            });
        }
    }

    // ========== 按钮点击处理 ==========

    void OnMonsterButtonClicked(int index)
    {
        currentMonsterIndex = index;

        // 填充详情信息
        FillMonsterDetail(index);

        // 打开详情页
        OpenMonsterDetail();
    }

    void OpenMonsterDetail()
    {
        if (monsterPanel != null)
            monsterPanel.SetActive(false);

        if (monsterDetailPanel != null)
        {
            monsterDetailPanel.SetActive(true);
        }

        // 禁用怪物面板的按钮（只有最上面面板能按）
        SetMonsterPanelButtons(false);
    }

    void OnCloseDetailClicked()
    {
        // 关闭所有面板，回到主页
        if (GalleryManager.Instance != null)
        {
            GalleryManager.Instance.CloseAllToHome();
        }
    }

    void OnBackDetailClicked()
    {
        // 关闭详情，打开怪物列表面板
        if (monsterDetailPanel != null)
            monsterDetailPanel.SetActive(false);

        if (monsterPanel != null)
        {
            monsterPanel.SetActive(true);
            // 重新启用怪物面板的按钮
            SetMonsterPanelButtons(true);
        }
    }

    void OnBackToGalleryClicked()
    {
        // 返回图鉴主面板（选择面具/怪物的那个面板）
        if (GalleryManager.Instance != null)
            GalleryManager.Instance.BackToGalleryMain();
    }

    void OnCloseMonsterPanelClicked()
    {
        // 关闭所有面板，回到主页
        if (GalleryManager.Instance != null)
            GalleryManager.Instance.CloseAllToHome();
    }

    // ========== 模态控制方法 ==========

    // 设置怪物面板按钮的可用状态
    void SetMonsterPanelButtons(bool enabled)
    {
        // 禁用/启用7个怪物按钮
        foreach (Button btn in monsterButtons)
        {
            if (btn != null)
                btn.interactable = enabled;
        }

        // 禁用/启用面板上的导航按钮
        if (backToGalleryButton != null)
            backToGalleryButton.interactable = enabled;

        if (closeMonsterPanelButton != null)
            closeMonsterPanelButton.interactable = enabled;
    }

    // ========== 详情页功能 ==========

    void FillMonsterDetail(int index)
    {
        if (index < 0 || index >= monsterNames.Length) return;

        // 1. 名称
        if (monsterName != null)
            monsterName.text = monsterNames[index];

        // 2. 图标（如果你有对应的Sprite，可以从数组或Manager获取）
        // if (monsterIcon != null && monsterSprites[index] != null)
        //     monsterIcon.sprite = monsterSprites[index];

        // 3. 详细描述（策划案信息）
        if (monsterSpecial != null)
            monsterSpecial.text = monsterDescs[index];

        // 4. 如果有单独的数据字段，可以解析 descs 或单独设置
        // 这里为了简单，将所有信息放在 special 文本中显示
        // 如果你想分开显示，可以解析字符串或改用结构化数据
    }
}