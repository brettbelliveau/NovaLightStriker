using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreRecap : MonoBehaviour {

    public GameObject BG, Complete, Score, TimeBonus, Total, ScoreVal, TimeVal, TotalVal;
    private int counter;
    public bool run;
    private float timeBonus, parTime;
    
	// Use this for initialization
	void Start () {
        counter = 0;
        run = false;
	}

    // Update is called once per frame
    void Update()
    {
        if (run)
            counter++;

        if (counter == 1)
        {
            Complete.SetActive(true);
            ScoreVal.GetComponent<Text>().text = Convert.ToString(Player.score);

            if (Player.currentLevel == 1)
            {
                parTime = 360; //360 seconds == 6 minutes
            }
            else if (Player.currentLevel == 2)
            {
                parTime = 240; //240 seconds == 4 minutes
            }
            else
            {
                parTime = 180; //180 seconds == 3 minutes
            }

            timeBonus = ((Player.finishTime - Player.awakeTime) - parTime) *100;

            TimeVal.GetComponent<Text>().text = Convert.ToString(timeBonus);
            TotalVal.GetComponent<Text>().text = Convert.ToString(Player.score + timeBonus);
        }

        else if (counter == 50)
            BG.SetActive(true);

        else if (counter == 100)
        {
            Score.SetActive(true);
            ScoreVal.SetActive(true);
        }

        else if (counter == 150)
        {
            TimeBonus.SetActive(true);
            TimeVal.SetActive(true);
        }

        else if (counter == 200)
        {
            Total.SetActive(true);
        }

        else if (counter == 250)
        {
            TotalVal.SetActive(true);
        }

        else if (counter == 500)
        {
            //Set global variables and load next level
            PlayerPrefs.SetInt("Score", Player.score);
            PlayerPrefs.SetInt("TotalScore", Player.totalScore);
            PlayerPrefs.SetInt("ExtraLives", Player.extraLives + 1);
            PlayerPrefs.SetInt("CurrentLevel", Player.currentLevel + 1);
            PlayerPrefs.SetString("CheckPointOne", Player.checkPointOne ? "True" : "False");
            PlayerPrefs.SetString("CheckPointTwo", Player.checkPointTwo ? "True" : "False");
            GameObject.FindObjectOfType<ScreenFader>().EndScene(Player.currentLevel + 1);
        }
    }
}
