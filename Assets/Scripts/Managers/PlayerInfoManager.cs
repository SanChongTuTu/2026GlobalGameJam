using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoManager : MonoBehaviour
{
    private static PlayerInfoManager instance;
    public static PlayerInfoManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance=FindObjectOfType<PlayerInfoManager>();
                if (instance == null)
                {
                    Debug.Log("No PlayerInfoManager!");
                }
            }
            return instance;
        }
    }
    public GameObject infos;
    public GameObject skilltips;
    public UnityEngine.UI.Image playericon;
    public TextMeshProUGUI maskname;
    public TextMeshProUGUI attacknum;
    public List<TextMeshProUGUI> tags=new List<TextMeshProUGUI>(4);

    public UnityEngine.UI.Image bloodback1;
    public UnityEngine.UI.Image bloodbarquick1;
    public UnityEngine.UI.Image bloodbarslow1;
    public TextMeshProUGUI bloodtext;

    [SerializeField] private float fadetime;
    public UnityEngine.UI.Image SkillIcon;
    public UnityEngine.UI.Image SkillCoolDownImage;
    public UnityEngine.UI.Image ChangeMaskCoolImage;

    public List<Sprite> maskicons=new List<Sprite>(6);
    public List<Sprite> skillicons;
    public List<Color> tagcolors;


    public float saveaimtime;
    public float savemasktime;

    private float maxsizex;

    void Start()
    {
        maxsizex = 310;
        bloodbarquick1.rectTransform.sizeDelta = new Vector2(maxsizex, bloodbarquick1.rectTransform.sizeDelta.y);
        bloodbarslow1.rectTransform.sizeDelta = new Vector2(maxsizex, bloodbarslow1.rectTransform.sizeDelta.y);
        bloodtext.text = "100/100";

        SkillCoolDownImage.gameObject.SetActive(false);
        ChangeMaskCoolImage.gameObject.SetActive(false);
        playericon.sprite = null;
        maskname.text="";
        attacknum.text = "";
        tags[0].text = "";
        tags[1].text = "";
        tags[2].text = "";
        tags[3].text = " ";
    }


    void Update()
    {
        float sizex=GameDataManager.Instance.health / 100f * maxsizex;
        if(sizex != bloodbarquick1.rectTransform.sizeDelta.x)
        {
            UpdateBloodBar();
        }
        if ((int)GameDataManager.Instance.playerType - 1>=0)
        SkillIcon.sprite=skillicons[(int)GameDataManager.Instance.playerType-1];

        switch (GameDataManager.Instance.playerType)
        {
            case GameDataManager.Type.none:
                playericon.sprite = null;
                maskname.text = "";
                attacknum.text = "";
                tags[0].text = "";
                tags[1].text = "";
                tags[2].text = "";
                tags[3].text = " ";
                break;
            case GameDataManager.Type.iron:
                playericon.sprite = maskicons[0];
                maskname.text = "<color=black>黑铁面具";
                attacknum.text = "5";
                tags[0].text = "近战";
                tags[0].color = tagcolors[0];
                tags[1].text = "击退";
                tags[1].color = tagcolors[2];
                tags[2].text = "";
                tags[3].text = " ";
                break;
            case GameDataManager.Type.fire:
                playericon.sprite = maskicons[1];
                maskname.text = "<color=red>火焰面具";
                attacknum.text = "5";
                tags[0].text = "远程";
                tags[0].color = tagcolors[0];
                tags[1].text = "";
                tags[2].text = "";
                tags[3].text = " ";
                break;
            case GameDataManager.Type.ice:
                playericon.sprite = maskicons[2];
                maskname.text = "<color=blue>寒冰面具";
                attacknum.text = "4";
                tags[0].text = "远程";
                tags[0].color = tagcolors[0];
                tags[1].text = "控制";
                tags[1].color = tagcolors[1];
                tags[2].text = "";
                tags[3].text = "";
                break;
            case GameDataManager.Type.wind:
                playericon.sprite = maskicons[3];
                maskname.text = "<color=green>疾风面具";
                attacknum.text = "5";
                tags[0].text = "近战";
                tags[0].color = tagcolors[0];
                tags[1].text = "";
                tags[2].text = "";
                tags[3].text = " ";
                break;
            case GameDataManager.Type.thunder:
                playericon.sprite = maskicons[4];
                maskname.text = "<color=yellow>雷霆面具";
                attacknum.text = "3";
                tags[0].text = "远程";
                tags[0].color = tagcolors[0];
                tags[1].text = "击退";
                tags[1].color = tagcolors[2];
                tags[2].text = "控制";
                tags[2].color = tagcolors[1];
                tags[3].text = "爆发";
                tags[3].color = tagcolors[3];
                break;
            case GameDataManager.Type.death:
                playericon.sprite = maskicons[5];
                maskname.text = "<color=white>死亡面具";
                attacknum.text = "";
                tags[0].text = "";
                tags[1].text = "";
                tags[2].text = "";
                tags[3].text = " ";
                break;
        }
    }

    public void SkillCoolDown(float time, float time2)
    {
        SkillCoolDownImage.gameObject.SetActive(true);
        saveaimtime = time;
        SkillCoolDownImage.fillAmount = 1;
        StartCoroutine(Cooldown(time));
        ChangeMaskCoolImage.gameObject.SetActive(true);
        savemasktime = time2;
        ChangeMaskCoolImage.fillAmount = 1;
        StartCoroutine(CooldownMask(time2));

    }

    IEnumerator CooldownMask(float aimtime)
    {
        while (savemasktime > 0)
        {
            savemasktime -= Time.deltaTime;
            ChangeMaskCoolImage.fillAmount = Mathf.Lerp(0, 1, savemasktime/ aimtime);
            yield return null;
        }
        ChangeMaskCoolImage.gameObject.SetActive(false);
        savemasktime = 0;
        ChangeMaskCoolImage.fillAmount = 0;
    }

    IEnumerator Cooldown(float aimtime)
    {
        while (saveaimtime > 0)
        {
            saveaimtime-=Time.deltaTime;
            SkillCoolDownImage.fillAmount = Mathf.Lerp(0, 1, saveaimtime / aimtime);
            yield return null;
        }
        SkillCoolDownImage.gameObject.SetActive(false);
        saveaimtime = 0;
        SkillCoolDownImage.fillAmount = 0;
    }

    void UpdateBloodBar()
    {
        bloodbarquick1.rectTransform.sizeDelta = new Vector2(GameDataManager.Instance.health / 100f * maxsizex, bloodbarquick1.rectTransform.sizeDelta.y);
        bloodtext.text=$"{GameDataManager.Instance.health}/100";
        StartCoroutine(Updatebloodbar());
    }

    IEnumerator Updatebloodbar()
    {
        float nowsizex=bloodbarslow1.rectTransform.sizeDelta.x;
        float time = 0;
        while(time<fadetime)
        {
            time+=Time.deltaTime;
            float percent = time / fadetime;
            float sizex = Mathf.Lerp(nowsizex, bloodbarquick1.rectTransform.sizeDelta.x, percent);
            bloodbarslow1.rectTransform.sizeDelta = new Vector2(sizex, bloodbarslow1.rectTransform.sizeDelta.y);
            yield return null;
        }
        bloodbarslow1.rectTransform.sizeDelta = new Vector2(GameDataManager.Instance.health / 100f * maxsizex, bloodbarslow1.rectTransform.sizeDelta.y);
        yield break;
    }
}
