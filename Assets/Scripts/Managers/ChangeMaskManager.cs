using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeMaskManager : MonoBehaviour
{
    public GameObject allpanel;
    public UnityEngine.UI.Button[] ChangeBtn = new UnityEngine.UI.Button[6];
    public Button ok;

    Image[] frame=new Image[6];
    Image[] question=new Image[6];

    private static ChangeMaskManager instance;
    public static ChangeMaskManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance=FindObjectOfType<ChangeMaskManager>();
                if(instance == null)
                {
                    Debug.Log("No ChangeMaskManager!");
                }
            }
            return instance;
        }
    }
    

    void Start()
    {
        allpanel.SetActive(false);
        for(int i = 0; i <= 5; i++)
        {
            frame[i] = ChangeBtn[i].transform.GetChild(3).GetComponent<Image>();
            question[i] = ChangeBtn[i].transform.GetChild(2).GetComponent<Image>();
            question[i].gameObject.SetActive(false);
            frame[i].color = Color.white;
        }

        ok.interactable = false;

        for (int i = 0; i <= 5; i++)
        {
            int savei = i;
            ChangeBtn[savei].onClick.AddListener(() =>
            {
                for(int j = 0; j <= 5; j++)
                {
                    if (j == savei)
                    {
                        frame[j].color = Color.red;
                    }
                    else
                    {
                        frame[j].color = Color.white;
                    }
                }

                ok.onClick.RemoveAllListeners();
                ok.onClick.AddListener(() =>
                {
                    Time.timeScale = 1;
                    PlayerInfoManager.Instance.infos.SetActive(true);
                    PlayerInfoManager.Instance.skilltips.SetActive(true);
                    foreach (GameObject go in FindObjectsOfType<GameObject>())
                    {
                        if (go.name == "Fireball(Clone)"|| go.name == "Ghost")
                            Destroy(go);
                    }
                    if (GameDataManager.Instance.playerType == GameDataManager.Type.ice)
                    {
                        IceMask ice = FindObjectOfType<IceMask>();
                        ice.EndLine();
                    }
                    GameDataManager.Instance.ChangeMask(savei + 1);
                    GameDataManager.Instance.player.GetComponent<BasicControl>().StopAction(0);
                });
                ok.interactable = true;
            });
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)&&PlayerInfoManager.Instance.savemasktime<=0&&!GameDataManager.Instance.banE)
        {
            ChangeMask();
        }
    }

    public void ReturnTime()
    {
        Time.timeScale = 1;
    }

    public void ChangeMask()
    {
        allpanel.SetActive(true);
        for (int i = 0; i <= 5; i++)
        {
            frame[i] = ChangeBtn[i].transform.GetChild(3).GetComponent<Image>();
            frame[i].color = Color.white;
            bool have = (PlayerPrefs.GetInt($"Mask{i + 1}", 0) == 1);
            if (have)
            {
                ChangeBtn[i].interactable = true;
                question[i].gameObject.SetActive(false);
            }
            else
            {
                ChangeBtn[i].interactable = false;
                question[i].gameObject.SetActive(true);
            }
        }
        Time.timeScale = 0;
        ok.interactable = false;
    }
}
