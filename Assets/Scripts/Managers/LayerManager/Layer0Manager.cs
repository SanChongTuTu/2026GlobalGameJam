using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer0Manager : LayerManager
{
    void Start()
    {
        Failure.SetActive(false);
        Next.SetActive(false);
        PlayerInfoManager.Instance.infos.SetActive(false);
        PlayerInfoManager.Instance.skilltips.SetActive(false);
        foreach (var btn in homes)
        {
            btn.onClick.AddListener(() => Scenemanager.Instance.ToScene(0));
        }
        foreach (var btn in nexts)
        {
            btn.onClick.AddListener(() => Scenemanager.Instance.ToScene(2));
        }
        restart.onClick.AddListener(() => Scenemanager.Instance.ToScene(1));
        foreach (var drops in dropmasks)
        {
            if (PlayerPrefs.GetInt($"Mask{drops.maskid}", 0) == 1)
            {
                Destroy(drops.gameObject);
            }
        }
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

        if (FindObjectOfType<SwordSkeleton>() == null)
        {
            GameDataManager.Instance.banL = true;
            GameDataManager.Instance.banE = true;
            GameDataManager.Instance.banJ = true;
            GameDataManager.Instance.player.GetComponent<BasicControl>().enabled = false;
            GameDataManager.Instance.player.GetComponent<JumpController>().enabled = false;
            finish.GetComponent<Animator>().SetTrigger("Next");
            PlayerPrefs.SetInt("Layer2", 1);
            PlayerPrefs.Save();
        }
    }
}
