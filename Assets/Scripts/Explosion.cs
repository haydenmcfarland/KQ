using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

    private AudioClip clipExplode;
    private AudioSource clipPlayer;

	// Use this for initialization
	void Start () {
        clipPlayer = GetComponent<AudioSource>();
        clipExplode = Resources.Load<AudioClip>("Sounds/8bit_bomb_explosion");
        StartCoroutine(LifeTime());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator LifeTime()
    {
        clipPlayer.PlayOneShot(clipExplode);
        yield return new WaitForSeconds(0.5f);
        if (gameObject != null)
            Destroy(gameObject);
    }
}
