using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer0Manager : MonoBehaviour
{
    public GameObject finish;
    public GameObject Failure;
    public GameObject Next;


    void Start()
    {
        Failure.SetActive(false);
        Next.SetActive(false);
        PlayerInfoManager.Instance.infos.SetActive(false);
        PlayerInfoManager.Instance.skilltips.SetActive(false);
    }


    void Update()
    {

        if (GameDataManager.Instance.health <= 0)
        {
            GameDataManager.Instance.banL = true;
            GameDataManager.Instance.banE = true;
            GameDataManager.Instance.player.GetComponent<BasicControl>().enabled = false;
            GameDataManager.Instance.player.GetComponent<JumpController>().enabled = false;
            finish.GetComponent<Animator>().SetTrigger("Lose");
            return;
        }

        if (FindObjectOfType<SwordSkeleton>() == null)
        {
            GameDataManager.Instance.banL = true;
            GameDataManager.Instance.banE = true;
            GameDataManager.Instance.player.GetComponent<BasicControl>().enabled = false;
            GameDataManager.Instance.player.GetComponent<JumpController>().enabled = false;
            finish.GetComponent<Animator>().SetTrigger("Next");
        }
    }
}
