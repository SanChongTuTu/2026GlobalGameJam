using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class LayerManager : MonoBehaviour
{
    public GameObject finish;
    public GameObject Failure;
    public GameObject Next;
    public List<MaskDrop> dropmasks;
    public List<Button> homes;
    public Button restart;
    public List<Button> nexts;
    public bool startmonster = false;

    private static LayerManager instance;
    public static LayerManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LayerManager>();
                if (instance == null)
                {
                    Debug.Log("No LayerManager found!");
                }
            }
            return instance;
        }
    }

    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
