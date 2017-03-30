using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour {

    private Text text;
    private int counter, waitCounter, waitBefore, waitAfter, index;
    private int framesPerChar;
    private string str;
    private bool run;

    // Use this for initialization
    void Start () {
        text = gameObject.GetComponent<Text>();
        text.text = "";
        counter = waitCounter = index = 0;
        run = false;
	}
	
	// Update is called once per frame
	void Update () {

        if (Time.timeScale != 1)
            return;

        if (run)
        {
            //Text has not been written, and we have not waited
            if (waitCounter < waitBefore)
            {
                waitCounter++;
            }

            //Text has not been written
            else if (counter > 0 || text.text == "")
            {
                text.enabled = true;
                waitCounter = waitBefore = 0;

                if (counter % framesPerChar == 0)
                {
                    text.text += str[index++];
                }
                counter = (counter + 1) % (str.Length * framesPerChar);
            }
            
            //Text has been written, and we have not waited
            else if (waitCounter < waitAfter)
            {
                waitCounter++;
            }

            //Text has been written and we have waited
            else
            {
                Player.stopMovement = false;
                text.enabled = false;
                text.text = "";
                run = false;
                counter = waitCounter = index = 0;
            }
        } 
	}

    public void writeText(string str, int waitBefore, int waitAfter, int framesPerChar) 
    {
        this.str = str;
        this.waitBefore = waitBefore;
        this.waitAfter = waitAfter;
        this.framesPerChar = framesPerChar;
        run = true;
    }
}
