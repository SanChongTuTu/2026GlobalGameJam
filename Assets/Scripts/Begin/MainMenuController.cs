using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button galleryButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private GameObject galleryPanel;
    [SerializeField] private GameObject settingsPanel;

    [SerializeField] private Button tutorialCloseButton;
    [SerializeField] private Button galleryCloseButton;
    [SerializeField] private Button settingsCloseButton;

    void Start()
    {
        // 绑定主菜单按钮
        if (startButton != null) startButton.onClick.AddListener(StartGame);
        if (tutorialButton != null) tutorialButton.onClick.AddListener(OpenTutorial);
        if (galleryButton != null) galleryButton.onClick.AddListener(OpenGallery);
        if (settingsButton != null) settingsButton.onClick.AddListener(OpenSettings);
        if (quitButton != null) quitButton.onClick.AddListener(QuitGame);

        // 绑定关闭按钮
        if (tutorialCloseButton != null)
            tutorialCloseButton.onClick.AddListener(CloseTutorial);

        if (galleryCloseButton != null)
            galleryCloseButton.onClick.AddListener(CloseGallery);

        if (settingsCloseButton != null)
            settingsCloseButton.onClick.AddListener(CloseSettings);

        // 初始化：隐藏所有面板
        HideAllPanels();
    }

    void StartGame()
    {
        
    }

    void OpenTutorial()
    {
        Debug.Log("打开教程面板");
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
            SetMainButtonsInteractable(false);
        }
    }

    void OpenGallery()
    {
        Debug.Log("打开图鉴面板");
        if (galleryPanel != null)
        {
            galleryPanel.SetActive(true);
            SetMainButtonsInteractable(false);
        }
    }

    void OpenSettings()
    {
        Debug.Log("打开设置面板");
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            SetMainButtonsInteractable(false);
        }
    }

    // 关闭按钮功能
    void CloseTutorial()
    {
        Debug.Log("关闭教程面板");
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
            SetMainButtonsInteractable(true);
        }
    }

    void CloseGallery()
    {
        Debug.Log("关闭图鉴面板");
        if (galleryPanel != null)
        {
            galleryPanel.SetActive(false);
            SetMainButtonsInteractable(true);
        }
    }

    void CloseSettings()
    {
        Debug.Log("关闭设置面板");
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            SetMainButtonsInteractable(true);
        }
    }

    // 修改为public，让其他脚本可以调用
    public void SetMainButtonsInteractable(bool interactable)
    {
        if (startButton != null) startButton.interactable = interactable;
        if (tutorialButton != null) tutorialButton.interactable = interactable;
        if (galleryButton != null) galleryButton.interactable = interactable;
        if (settingsButton != null) settingsButton.interactable = interactable;
        if (quitButton != null) quitButton.interactable = interactable;
    }

    // 添加缺少的OnTutorialClosed方法
    public void OnTutorialClosed()
    {
        Debug.Log("接收到教程关闭通知");
        SetMainButtonsInteractable(true);
    }

    void HideAllPanels()
    {
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
        if (galleryPanel != null) galleryPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    void QuitGame()
    {
        Application.Quit();
    }
}