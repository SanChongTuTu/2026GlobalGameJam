using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseLayerManager : MonoBehaviour
{
    public GameObject allpanel;
    public UnityEngine.UI.Button[] LayerBtn = new UnityEngine.UI.Button[5];

    GameObject[] question=new GameObject[5];

    private static ChooseLayerManager instance;
    public static ChooseLayerManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance=FindObjectOfType<ChooseLayerManager>();
                if(instance == null)
                {
                    Debug.Log("No ChooseLayerManager!");
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        PlayerPrefs.SetInt("Layer1", 1);
        PlayerPrefs.Save();
        allpanel.SetActive(false);
        for (int i = 0; i <= 4; i++)
        {
            LayerBtn[i].interactable=PlayerPrefs.GetInt("Layer" + (i + 1), 0) == 1;
            question[i] = LayerBtn[i].transform.GetChild(3).gameObject;
            question[i].gameObject.SetActive(PlayerPrefs.GetInt("Layer" + (i + 1), 0) == 0);
            int dexi = i;
            LayerBtn[i].onClick.AddListener(()=>Scenemanager.Instance.ToScene(dexi+1));
        }
    }


    public void OpenLayer()
    {
        allpanel.SetActive(true);
        for (int i = 0; i <= 4; i++)
        {
            LayerBtn[i].interactable = PlayerPrefs.GetInt("Layer" + (i + 1), 0) == 1;
            question[i] = LayerBtn[i].transform.GetChild(3).gameObject;
            question[i].gameObject.SetActive(PlayerPrefs.GetInt("Layer" + (i + 1), 0) == 0);
        }
    }

}
