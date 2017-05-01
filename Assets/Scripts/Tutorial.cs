using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour {

    public Text timer;
    public GameObject gem1;
    public TextAsset tutorial;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        timer.GetComponent<timer>().time = 0;
		if (Input.GetKeyDown(KeyCode.Return))
        {
            Instantiate(gem1, gameObject.transform.position, gameObject.transform.rotation);
            if (gameObject != null)
                Destroy(gameObject);
        }
	}
}
