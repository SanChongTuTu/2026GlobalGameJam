using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Transmit : MonoBehaviour
{
    public GameObject airWall;
    public GameObject monster;
    public UnityEvent events;
    // Start is called before the first frame update
    void Start()
    {
        airWall.SetActive(false);
        monster.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            LayerManager.Instance.startmonster = true;
            airWall.SetActive(true);
            monster.SetActive(true);
            Destroy(gameObject);
        }

        events?.Invoke();
    }
}
