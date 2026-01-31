using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AttackRangeTrigger : MonoBehaviour
{
    public int id;
    private Monster _monster;

    void Start()
    {
        _monster = transform.parent.GetComponent<Monster>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (id == 0) // MELEE
            {
                transform.parent.GetComponent<Monster>()?.Attack(other, id);
                enabled = false;
            }
            else if (id == 1) // RANGED
            {
                transform.parent.GetComponent<Vampire>()?.SetPlayerInRange(true, id, other);
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _monster.SetPlayerInRange(false, id, null);
        }
    }

}
