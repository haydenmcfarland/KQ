using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class TextDisplay : MonoBehaviour
{

    public float letterPause = 0.2f;
    public AudioSource typeSound;

    string message;
    private string[] messages;
    Text textComp;
    public Text pressEnterText;
    public bool done = false;
    public TextAsset textAsset;

    // Use this for initialization
    void Start()
    {
        var lines = textAsset.text.Split("\n"[0]);

        pressEnterText.enabled = false;
        textComp = GetComponent<Text>();
        messages = lines;
        textComp.text = "";
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        foreach (string message in messages)
        {
            foreach (char letter in message.ToUpper().ToCharArray())
            {
                textComp.text += letter;
                if (typeSound)
                {
                    typeSound.Play();
                    yield return 0;
                }

                yield return new WaitForSeconds(letterPause);
            }

            pressEnterText.enabled = true;
            while (!Input.GetKeyDown(KeyCode.Return))
                yield return null;
            pressEnterText.enabled = false;
            textComp.text = "";
        }
        done = true;
    }

}