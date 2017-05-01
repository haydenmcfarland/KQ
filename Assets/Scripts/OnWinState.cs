using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnWinState : MonoBehaviour {

    public GameObject gc;
    public GameObject player;
    public Text Timer;
    public Text Score;
    public Text Health;
    public Text Time;
    public Text Treasures;
    bool done = false;
    bool loading = false;

	// Use this for initialization
	void Start ()
    {   
       
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (player.GetComponent<PlayerController>().getWinState() && !done)
        {
            Timer.GetComponent<timer>().Stop();
            float time = Timer.GetComponent<timer>().time;
            int treasures = 0;
            if (player.GetComponent<PlayerController>().treasure_one.enabled)
                treasures += 1;
            if (player.GetComponent<PlayerController>().treasure_two.enabled)
                treasures += 1;
            if (player.GetComponent<PlayerController>().treasure_three.enabled)
                treasures += 1;
            float health_remaining = player.GetComponent<PlayerController>().health_percent;
            float score = health_remaining * 1000 + treasures * 10000 - time*10;



            Health.text += string.Format("{000}", Mathf.Round(health_remaining));
            Treasures.text += treasures.ToString();
            Time.text += Timer.GetComponent<timer>().getFormattedTime();
            Score.text += Mathf.Round(score).ToString();

            done = true;
        }
        else if (done && Input.GetKeyDown(KeyCode.Return) && !loading)
        {
            GetComponent<Exit>().exit();
        }
        else if (done && Input.GetKeyDown(KeyCode.E) && !loading)
        {
            gc.GetComponent<Change_Level>().LoadScene("test");
            loading = true;
        }
        
    }
}
