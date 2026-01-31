using UnityEngine;
using UnityEngine.UI;

public class MaskController : MonoBehaviour
{
    [Header("面板引用")]
    public GameObject maskPanel;
    public GameObject maskDetailPanel;

    [Header("6个面具按钮")]
    public Button[] maskButtons = new Button[6];

    [Header("详情页按钮")]
    public Button closeDetailButton;
    public Button backDetailButton;

    [Header("面具面板按钮")]
    public Button backToGalleryButton;
    public Button closeMaskPanelButton;

    [Header("详情页UI组件")]
    public Image detailIcon;
    public Text detailName;
    public Text detailDescription;
    public Text detailAttack;
    public Text detailSkill;
    public Text detailSpecial;
    public Text detailUnlock;

    // 当前选择的面具索引
    private int currentMaskIndex = -1;

    void Start()
    {
        if (maskDetailPanel != null)
            maskDetailPanel.SetActive(false);

        SetupButtonEvents();
        Debug.Log("MaskController初始化完成");
    }

    void SetupButtonEvents()
    {
        // 1. 6个面具按钮
        for (int i = 0; i < maskButtons.Length; i++)
        {
            if (maskButtons[i] == null) continue;

            int index = i;
            maskButtons[i].onClick.RemoveAllListeners();
            maskButtons[i].onClick.AddListener(() =>
            {
                OnMaskButtonClicked(index);
            });
        }

        // 2. 详情页关闭按钮
        if (closeDetailButton != null)
        {
            closeDetailButton.onClick.RemoveAllListeners();
            closeDetailButton.onClick.AddListener(() =>
            {
                OnCloseDetailClicked();
            });
        }

        // 3. 详情页返回按钮
        if (backDetailButton != null)
        {
            backDetailButton.onClick.RemoveAllListeners();
            backDetailButton.onClick.AddListener(() =>
            {
                OnBackDetailClicked();
            });
        }

        // 4. 面具面板返回按钮
        if (backToGalleryButton != null)
        {
            backToGalleryButton.onClick.RemoveAllListeners();
            backToGalleryButton.onClick.AddListener(() =>
            {
                OnBackToGalleryClicked();
            });
        }

        // 5. 面具面板关闭按钮
        if (closeMaskPanelButton != null)
        {
            closeMaskPanelButton.onClick.RemoveAllListeners();
            closeMaskPanelButton.onClick.AddListener(() =>
            {
                OnCloseMaskPanelClicked();
            });
        }
    }

    void OnMaskButtonClicked(int index)
    {
        currentMaskIndex = index;

        // 获取面具数据
        CreateMask mask = GetMaskDataByIndex(index);
        if (mask == null)
        {
            Debug.LogError($"无法获取面具数据，索引: {index}");
            return;
        }

        // 填充详情信息
        FillMaskDetail(mask);

        // 打开详情页
        OpenMaskDetail();
    }

    void OpenMaskDetail()
    {
        
        if (maskDetailPanel != null)
        {
            maskDetailPanel.SetActive(true);
        }

        
        if (maskPanel != null)
            maskPanel.SetActive(false);

        
        SetMaskPanelButtons(false);

       
        if (detailName != null) detailName.SetAllDirty();
        if (detailDescription != null) detailDescription.SetAllDirty();
        if (detailAttack != null) detailAttack.SetAllDirty();
        if (detailSkill != null) detailSkill.SetAllDirty();
        if (detailSpecial != null) detailSpecial.SetAllDirty();
        if (detailUnlock != null) detailUnlock.SetAllDirty();
    }
    void OnCloseDetailClicked()
    {
        // 使用GalleryManager关闭所有面板
        if (GalleryManager.Instance != null)
        {
            GalleryManager.Instance.CloseAllToHome();
        }
    }

    void OnBackDetailClicked()
    {
        // 关闭详情，打开列表
        if (maskDetailPanel != null)
            maskDetailPanel.SetActive(false);

        if (maskPanel != null)
        {
            maskPanel.SetActive(true);
            // 重新启用面具面板的按钮
            SetMaskPanelButtons(true);
        }
    }

    void OnBackToGalleryClicked()
    {
        if (GalleryManager.Instance != null)
            GalleryManager.Instance.BackToGalleryMain();
    }

    void OnCloseMaskPanelClicked()
    {
        if (GalleryManager.Instance != null)
            GalleryManager.Instance.CloseAllToHome();
    }

  
    void SetMaskPanelButtons(bool enabled)
    {
        foreach (Button btn in maskButtons)
        {
            if (btn != null)
                btn.interactable = enabled;
        }

        if (backToGalleryButton != null)
            backToGalleryButton.interactable = enabled;

        if (closeMaskPanelButton != null)
            closeMaskPanelButton.interactable = enabled;
    }

    
    CreateMask GetMaskDataByIndex(int index)
    {
        if (MaskManager.Instance == null)
        {
            Debug.LogError("MaskManager未找到！");
            return null;
        }

        switch (index)
        {
            case 0: return MaskManager.Instance.GetMaskByName(CreateMask.MaskType.黑铁面具);
            case 1: return MaskManager.Instance.GetMaskByName(CreateMask.MaskType.火焰面具);
            case 2: return MaskManager.Instance.GetMaskByName(CreateMask.MaskType.疾风面具);
            case 3: return MaskManager.Instance.GetMaskByName(CreateMask.MaskType.寒冰面具);
            case 4: return MaskManager.Instance.GetMaskByName(CreateMask.MaskType.雷霆面具);
            case 5: return MaskManager.Instance.GetMaskByName(CreateMask.MaskType.死亡面具);
            default:
                Debug.LogError($"无效的面具索引: {index}");
                return null;
        }
    }

    void FillMaskDetail(CreateMask mask)
    {
        if (mask == null) return;

        // 1. 图标
        if (detailIcon != null && mask.icon != null)
            detailIcon.sprite = mask.icon;

        // 2. 名称
        if (detailName != null)
            detailName.text = mask.maskName.ToString();

        // 3. 描述
        if (detailDescription != null)
            detailDescription.text = mask.description;

        // 4. 攻击信息
        if (detailAttack != null)
        {
            string attackText = $"<b>普通攻击：</b>\n{mask.normalAttackDesc}";
            if (mask.attackBonus > 0)
                attackText += $"\n<b>攻击力加成：</b>+{mask.attackBonus}";
            detailAttack.text = attackText;
        }

        // 5. 技能信息
        if (detailSkill != null)
        {
            string skillText = $"<b>技能：</b>\n{mask.skillDesc}";
            if (mask.skillCooldown > 0)
                skillText += $"\n<b>冷却时间：</b>{mask.skillCooldown}秒";
            detailSkill.text = skillText;
        }

        // 6. 特殊效果
        if (detailSpecial != null && !string.IsNullOrEmpty(mask.specialEffectDesc))
            detailSpecial.text = $"<b>特殊效果：</b>\n{mask.specialEffectDesc}";

        // 7. 解锁条件
        if (detailUnlock != null && !string.IsNullOrEmpty(mask.unlockCondition))
            detailUnlock.text = $"<b>解锁条件：</b>\n{mask.unlockCondition}";
    }

    System.Collections.IEnumerator RefreshUI()
    {
        yield return null;

        if (detailName != null) detailName.SetAllDirty();
        if (detailDescription != null) detailDescription.SetAllDirty();
        if (detailAttack != null) detailAttack.SetAllDirty();
        if (detailSkill != null) detailSkill.SetAllDirty();
        if (detailSpecial != null) detailSpecial.SetAllDirty();
        if (detailUnlock != null) detailUnlock.SetAllDirty();
    }
}