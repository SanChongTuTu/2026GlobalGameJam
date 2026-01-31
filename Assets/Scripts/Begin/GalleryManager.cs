using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GalleryManager : MonoBehaviour
{
    // 所有面板
    public GameObject galleryMainPanel;
    public GameObject maskPanel;
    public GameObject monsterPanel;
    public GameObject maskDetailPanel;
    public GameObject monsterDetailPanel;

    // 主页的所有按钮（需要禁用的）
    public Button[] homePageButtons; // 拖拽主页的所有按钮

    // 当前打开的面板
    private GameObject currentOpenPanel = null;

    // 单例
    private static GalleryManager _instance;
    public static GalleryManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<GalleryManager>();
            return _instance;
        }
    }

    void Start()
    {
        // 确保所有图鉴面板初始关闭
        CloseAllGalleryPanels();
    }

    // ========== 公开方法 ==========

    // 从主页打开图鉴
    public void OpenGalleryFromHome()
    {
        // 禁用主页按钮
        SetHomePageButtons(false);

        CloseAllGalleryPanels();
        if (galleryMainPanel != null)
        {
            galleryMainPanel.SetActive(true);
            currentOpenPanel = galleryMainPanel;
            Debug.Log("打开图鉴主面板");
        }
    }

    // 打开面具图鉴
    public void OpenMaskPanel()
    {
        SetHomePageButtons(false);
        CloseAllGalleryPanels();
        if (maskPanel != null)
        {
            maskPanel.SetActive(true);
            currentOpenPanel = maskPanel;
            Debug.Log("打开面具图鉴面板");
        }
    }

    // 打开怪物图鉴
    public void OpenMonsterPanel()
    {
        SetHomePageButtons(false);
        CloseAllGalleryPanels();
        if (monsterPanel != null)
        {
            monsterPanel.SetActive(true);
            currentOpenPanel = monsterPanel;
            Debug.Log("打开怪物图鉴面板");
        }
    }

    // 打开面具详情
    public void OpenMaskDetail()
    {
        SetHomePageButtons(false);

        if (maskPanel != null) maskPanel.SetActive(false);
        if (maskDetailPanel != null)
        {
            maskDetailPanel.SetActive(true);
            currentOpenPanel = maskDetailPanel;
            Debug.Log("打开面具详情页");
        }
    }

    // 打开怪物详情
    public void OpenMonsterDetail()
    {
        SetHomePageButtons(false);

        if (monsterPanel != null) monsterPanel.SetActive(false);
        if (monsterDetailPanel != null)
        {
            monsterDetailPanel.SetActive(true);
            currentOpenPanel = monsterDetailPanel;
            Debug.Log("打开怪物详情页");
        }
    }

    // 从详情页返回列表
    public void BackFromDetail()
    {
        if (maskDetailPanel != null && maskDetailPanel.activeSelf)
        {
            maskDetailPanel.SetActive(false);
            if (maskPanel != null)
            {
                maskPanel.SetActive(true);
                currentOpenPanel = maskPanel;
            }
            Debug.Log("从面具详情返回列表");
        }
        else if (monsterDetailPanel != null && monsterDetailPanel.activeSelf)
        {
            monsterDetailPanel.SetActive(false);
            if (monsterPanel != null)
            {
                monsterPanel.SetActive(true);
                currentOpenPanel = monsterPanel;
            }
            Debug.Log("从怪物详情返回列表");
        }
    }

    // 从列表返回图鉴主面板
    public void BackToGalleryMain()
    {
        CloseAllGalleryPanels();
        if (galleryMainPanel != null)
        {
            galleryMainPanel.SetActive(true);
            currentOpenPanel = galleryMainPanel;
            Debug.Log("返回图鉴主面板");
        }
    }

    // 关闭所有图鉴，返回主页
    public void CloseAllToHome()
    {
        Debug.Log("关闭所有图鉴，返回主页");

       
        
        CloseAllGalleryPanels();
        currentOpenPanel = null;

        
        SetHomePageButtons(true);

        
    }


    void SetHomePageButtons(bool enabled)
    {
        bool hasProcessed = false;

        // 1. 优先使用预设的按钮数组（如果Inspector中设置了）
        if (homePageButtons != null && homePageButtons.Length > 0)
        {
            foreach (Button btn in homePageButtons)
            {
                if (btn != null)
                    btn.interactable = enabled;
            }
            hasProcessed = true;
            Debug.Log($"主页按钮: {(enabled ? "启用" : "禁用")} (通过数组设置 {homePageButtons.Length} 个按钮)");
        }

        // 2. 如果没设置数组，尝试自动查找（支持多种常见命名）
        if (!hasProcessed)
        {
            string[] possibleNames = new string[] { "HomePage", "MainPage", "Home", "MainMenu", "Menu", "StartPage", "Main" };
            GameObject homePage = null;

            foreach (string name in possibleNames)
            {
                homePage = GameObject.Find(name);
                if (homePage != null)
                {
                    Debug.Log($"找到主页对象: {name}");
                    break;
                }
            }

            if (homePage != null)
            {
                Button[] buttons = homePage.GetComponentsInChildren<Button>(true);
                foreach (Button btn in buttons)
                {
                    if (btn != null)
                        btn.interactable = enabled;
                }
            }
            else
            {
                Debug.LogWarning("未找到主页对象！请确保主页GameObject名为 'HomePage' 或在Inspector中设置 homePageButtons 数组");
            }
        }
    }

    // 检查是否有面板打开
    public bool IsAnyPanelOpen()
    {
        return currentOpenPanel != null;
    }

    // ========== 私有方法 ==========

    void CloseAllGalleryPanels()
    {
        if (galleryMainPanel != null) galleryMainPanel.SetActive(false);
        if (maskPanel != null) maskPanel.SetActive(false);
        if (monsterPanel != null) monsterPanel.SetActive(false);
        if (maskDetailPanel != null) maskDetailPanel.SetActive(false);
        if (monsterDetailPanel != null) monsterDetailPanel.SetActive(false);
    }
}