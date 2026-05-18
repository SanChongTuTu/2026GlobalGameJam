using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastLayerManager : LayerManager
{
    void Start()
    {
        Failure.SetActive(false);
        Next.SetActive(false);
        //PlayerPrefs.SetInt("Mask1", 1);
        //PlayerPrefs.SetInt("Mask2", 1);
        //PlayerPrefs.SetInt("Mask3", 1);
        //PlayerPrefs.SetInt("Mask4", 1);
        //PlayerPrefs.SetInt("Mask5", 1);
        //PlayerPrefs.SetInt("Mask6", 1);
        //PlayerPrefs.Save();
        foreach (var btn in homes)
        {
            btn.onClick.AddListener(() => Scenemanager.Instance.ToScene(0));
        }
        foreach (var btn in nexts)
        {

        }
        restart.onClick.AddListener(() => Scenemanager.Instance.ToScene(5));
        foreach (var drops in dropmasks)
        {
            if (PlayerPrefs.GetInt($"Mask{drops.maskid}", 0) == 1)
            {
                Destroy(drops.gameObject);
            }
        }
    }

    public void ChangeStart()
    {

    }


    void Update()
    {

        if (GameDataManager.Instance.health <= 0)
        {
            GameDataManager.Instance.banL = true;
            GameDataManager.Instance.banE = true;
            GameDataManager.Instance.banJ = true;
            GameDataManager.Instance.player.GetComponent<BasicControl>().enabled = false;
            GameDataManager.Instance.player.GetComponent<JumpController>().enabled = false;
            finish.GetComponent<Animator>().SetTrigger("Lose");
            return;
        }

        if (FindObjectOfType<Monster>() == null&&startmonster)
        {
            GameDataManager.Instance.banL = true;
            GameDataManager.Instance.banE = true;
            GameDataManager.Instance.banJ = true;
            GameDataManager.Instance.player.GetComponent<BasicControl>().enabled = false;
            GameDataManager.Instance.player.GetComponent<JumpController>().enabled = false;
            finish.GetComponent<Animator>().SetTrigger("Next");
        }
    }
}
