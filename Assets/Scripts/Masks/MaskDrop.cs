using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskDrop : MonoBehaviour
{
    public int maskid;
    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetNewMask()
    {
        PlayerPrefs.SetInt($"Mask{maskid}", 1);
        PlayerPrefs.Save();
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        anim.SetTrigger("Get");
    }
}
