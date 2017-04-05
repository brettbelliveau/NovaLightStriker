using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreRecap : MonoBehaviour {

    public GameObject BG, Complete, Score, TimeBonus, Total, ScoreVal, TimeVal, TotalVal, ExtraLives;
    public GameObject[] lives;
    private int counter, extraLives;
    public bool run;
    private float timeBonus, parTime;
    
	// Use this for initialization
	void Start () {
        counter = extraLives = 0;
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
            Debug.Log("Finish + " + Player.finishTime + ", Awake" + Player.awakeTime);
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
        
        else if (counter == 380)
        {
            if (Player.currentLevel < 3)
            {
                if (Player.currentLevel == 1)
                    extraLives = Player.totalScore / 20000;

                else //Player.currentLevel == 2
                    extraLives = Player.totalScore / 15000;

                extraLives = extraLives > 3 ? 3 : extraLives;
                Player.extraLives += extraLives;
                ExtraLives.SetActive(true);
            }

            else
            {
                counter = 450;
            }
        }

        else if (counter == 410)
        {
            if (extraLives >= 1)
            {
                lives[0].SetActive(true);
            }
        }

        else if (counter == 435)
        {
            if (extraLives >= 2)
            {
                lives[1].SetActive(true);
            }
        }

        else if (counter == 460)
        {
            if (extraLives == 3)
            {
                lives[2].SetActive(true);
            }
        }

        else if ((counter >= 450 && extraLives < 1) || (counter >= 540))
        {
            PlayerPrefs.DeleteAll();

            PlayerPrefs.SetInt("TotalScore", Player.totalScore);

            PlayerPrefs.SetInt("CurrentLevel", Player.currentLevel);

            if (PlayerPrefs.GetInt("CurrentLevel") == 1)
                PlayerPrefs.SetInt("LevelOneScore", Player.totalScore);

            else if (PlayerPrefs.GetInt("CurrentLevel") == 2)
                PlayerPrefs.SetInt("LevelTwoScore", Player.totalScore);

            else if (PlayerPrefs.GetInt("CurrentLevel") == 3)
                PlayerPrefs.SetInt("LevelThreeScore", Player.totalScore);
            
            PlayerPrefs.SetInt("ExtraLives", Player.extraLives);

            GameObject.FindObjectOfType<ScreenFader>().speed = 1;

            if (Player.currentLevel < 3)
                GameObject.FindObjectOfType<ScreenFader>().EndScene(Player.currentLevel + 1);
            else
                GameObject.FindObjectOfType<ScreenFader>().EndScene(6);
        }
    }
}
