using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenemanager : MonoBehaviour
{

    private static Scenemanager instance;
    public static Scenemanager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Scenemanager>();
                if (instance == null)
                {
                    Debug.Log("No Scenemanager");
                }
            }
            return instance;
        }
    }

    public enum NowScene
    {
        Start = 0,
        SkillTree = 1,
        Game1 = 2,
        Game2=3,
        Game3=4,
    }

    [Header("当前场景")]
    public NowScene nowscene;
    [Header("切换场景过渡遮罩")]
    public GameObject sceneshader;
    [Header("淡入时间")]
    public float fadeintime;
    [Header("中间态过渡时间")]
    public float fadetime;
    [Header("淡出时间")]
    public float fadeouttime;

    void Start()
    {
        sceneshader.SetActive(false);
        //sceneshader.GetComponent<CanvasGroup>().alpha = 0;
        nowscene=(NowScene)SceneManager.GetActiveScene().buildIndex;
    }

    void Update()
    {
        
    }

    public void ToScene(int id)
    {
        //GameDateController.Instance.tempAttackBonus = 0;
        sceneshader.SetActive(true);
        Time.timeScale = 1;
        sceneshader.GetComponent<CanvasGroup>().alpha = 0;
        //PlayerSet.Instance.setbtn = null;
        StartCoroutine(SceneShaderFade(id));
    }

    IEnumerator SceneShaderFade(int id)
    {
        CanvasGroup canvasGroup = sceneshader.GetComponent<CanvasGroup>();
        float time = 0;
        while (canvasGroup.alpha < 1)
        {
            time+= Time.deltaTime;
            canvasGroup.alpha=Mathf.Lerp(0,1, time / fadeintime);
            yield return null;
        }
        canvasGroup.alpha = 1;


        SceneManager.LoadScene(id);
        nowscene = (NowScene)id;
        //PlayerSet.Instance.ifbackhome.SetActive(false);
        //PlayerSet.Instance.sets.SetActive(false);
        yield return new WaitForSeconds(fadetime);
        time = 0;
        while (canvasGroup.alpha >0)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1,0, time / fadeouttime);
            yield return null;
        }
        canvasGroup.alpha = 0;
        sceneshader.SetActive(false);
        yield break;
    }
}
