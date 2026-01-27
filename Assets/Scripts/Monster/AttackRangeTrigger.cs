using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AttackRangeTrigger : MonoBehaviour
{
    public int id;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        UnityEngine.Debug.Log(other.name);
        if (other.CompareTag("Player"))
        {
            transform.parent.GetComponent<Monster>()?.Attack(other,id);
            enabled = false;
        }
    }

}
