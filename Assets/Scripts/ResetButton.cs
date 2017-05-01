using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetButton : MonoBehaviour {

    private bool reset = false;
    public GameObject button;
	// Use this for initialization
	void Start () {
        button = GameObject.Find("ResetButton");
        button.SetActive(false);

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            reset = !reset;
            button.SetActive(reset);
        }

    }
}
