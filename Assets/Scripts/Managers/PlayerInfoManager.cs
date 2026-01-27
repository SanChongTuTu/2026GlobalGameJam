using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerInfoManager : MonoBehaviour
{
    public UnityEngine.UI.Image bloodback;
    public UnityEngine.UI.Image bloodbarquick;
    public UnityEngine.UI.Image bloodbarslow;
    public TextMeshProUGUI bloodtext;
    [SerializeField] private float fadetime;

    private float maxsizex;

    void Start()
    {
        maxsizex = bloodback.rectTransform.sizeDelta.x;
        bloodbarquick.rectTransform.sizeDelta = new Vector2(maxsizex, bloodbarquick.rectTransform.sizeDelta.y);
        bloodbarslow.rectTransform.sizeDelta = new Vector2(maxsizex, bloodbarslow.rectTransform.sizeDelta.y);
        bloodtext.text = "100/100";
    }


    void Update()
    {
        float sizex=GameDataManager.Instance.health / 100f * maxsizex;
        if(sizex != bloodbarquick.rectTransform.sizeDelta.x)
        {
            UpdateBloodBar();
        }
    }

    void UpdateBloodBar()
    {
        bloodbarquick.rectTransform.sizeDelta = new Vector2(GameDataManager.Instance.health / 100f * maxsizex, bloodbarquick.rectTransform.sizeDelta.y);
        bloodtext.text=$"{GameDataManager.Instance.health}/100";
        StartCoroutine(Updatebloodbar());
    }

    IEnumerator Updatebloodbar()
    {
        float nowsizex=bloodbarslow.rectTransform.sizeDelta.x;
        float time = 0;
        while(time<fadetime)
        {
            time+=Time.deltaTime;
            float percent = time / fadetime;
            float sizex = Mathf.Lerp(nowsizex, bloodbarquick.rectTransform.sizeDelta.x, percent);
            bloodbarslow.rectTransform.sizeDelta = new Vector2(sizex, bloodbarslow.rectTransform.sizeDelta.y);
            yield return null;
        }
        bloodbarslow.rectTransform.sizeDelta = new Vector2(GameDataManager.Instance.health / 100f * maxsizex, bloodbarslow.rectTransform.sizeDelta.y);
        yield break;
    }
}
