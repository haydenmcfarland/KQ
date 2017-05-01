using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class timer : MonoBehaviour {

    public float time = 0;
    private bool stop = false;
    private string formattedTime = "";
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (!stop)
            time += Time.deltaTime;

        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time - minutes * 60);
        string timeString = string.Format("{0:00}:{1:00}", minutes, seconds);
        formattedTime = timeString;
        GetComponent<Text>().text = timeString;
    }

    public string getFormattedTime()
    {
        return formattedTime;
    }

    public void Stop()
    {
        stop = true;
    }
}
