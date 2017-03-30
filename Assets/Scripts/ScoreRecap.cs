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

            timeBonus = (parTime - (Player.finishTime - Player.awakeTime)) * 100;
            if (timeBonus > 0)
                timeBonus = Convert.ToInt64(Math.Round(Convert.ToDouble(timeBonus) / 100) * 100);
            else
                timeBonus = 0;
            
            TimeVal.GetComponent<Text>().text = Convert.ToString(timeBonus);
            TotalVal.GetComponent<Text>().text = Convert.ToString(Player.score + timeBonus);
            Player.totalScore = Player.score + Convert.ToInt32(timeBonus);
        }

        else if (counter == 30)
            BG.SetActive(true);

        else if (counter == 70)
        {
            Score.SetActive(true);
        }

        else if (counter == 100)
        {
            ScoreVal.SetActive(true);
        }

        else if (counter == 150)
        {
            TimeBonus.SetActive(true);
        }

        else if (counter == 180)
        {
            TimeVal.SetActive(true);
        }

        else if (counter == 240)
        {
            Total.SetActive(true);
        }

        else if (counter == 290)
        {
            TotalVal.SetActive(true);
        }

        //TODO: Add extra lives?
        else if (counter == 420)
        {
            if (PlayerPrefs.GetInt("CurrentLevel") == 1)
                PlayerPrefs.SetInt("LevelOneScore", Player.totalScore);

            else if (PlayerPrefs.GetInt("CurrentLevel") == 2)
                PlayerPrefs.SetInt("LevelTwoScore", Player.totalScore);

            else if (PlayerPrefs.GetInt("CurrentLevel") == 3)
                PlayerPrefs.SetInt("LevelThreeScore", Player.totalScore);
            
            int extraLives = (Player.totalScore + Convert.ToInt32(timeBonus)) / 10000;
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetInt("ExtraLives", Player.extraLives+extraLives);
            PlayerPrefs.SetInt("TotalScore", Player.totalScore);
            PlayerPrefs.SetInt("CurrentLevel", Player.currentLevel);
            if (Player.currentLevel < 3)
                GameObject.FindObjectOfType<ScreenFader>().EndScene(Player.currentLevel + 1);
            else
                GameObject.FindObjectOfType<ScreenFader>().EndScene(6);
        }
    }
}
