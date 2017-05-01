using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossBattle : MonoBehaviour
{

    public GameObject BarrierBoss;

    // Use this for initialization
    void Start()
    {
        BarrierBoss.GetComponent<ParticleSystem>().Stop();
        BarrierBoss.GetComponent<BoxCollider2D>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.Contains("Character"))
        {
            BarrierBoss.GetComponent<ParticleSystem>().Play();
            BarrierBoss.GetComponent<BoxCollider2D>().enabled = true;
        }
    }
}
