using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public Image one;
    public Image two;
    private bool play = true;

    private void Start()
    {
        two.enabled = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (play)
                gameObject.GetComponent<Change_Level>().LoadScene("test");
            else
                exit();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            play = !play;
            if (play && !one.enabled)
            {
                one.enabled = true;
                two.enabled = false;
            }
            else
            {
                one.enabled = false;
                two.enabled = true;
            }
        }




    }
    public void exit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif

        // Perform any game state save operations here
        // End state save operations
        Application.Quit();
    }
}
