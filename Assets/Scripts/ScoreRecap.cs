using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
            Player.fadeOutSound = true;
            Complete.SetActive(true);
            BG.SetActive(true);
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
                parTime = 300; //300 seconds == 5 minutes
            }

            timeBonus = (parTime - (Player.finishTime - Player.awakeTime)) * 100;
            Debug.Log("Finish + " + Player.finishTime + ", Awake" + Player.awakeTime);
            if (timeBonus > 0)
                timeBonus = Convert.ToInt64(Math.Round(Convert.ToDouble(timeBonus) / 100) * 100);
            else
                timeBonus = 0;
            
            TimeVal.GetComponent<Text>().text = Convert.ToString(timeBonus);
            TotalVal.GetComponent<Text>().text = Convert.ToString(Player.score + timeBonus);
            Player.totalScore += Convert.ToInt32(timeBonus);
        }
        
        else if (counter == 60 || counter == 90 || counter == 140 
            || counter == 170 || counter == 230 || counter == 280)
            gameObject.GetComponent<AudioSource>().Play();

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
                if (Player.currentLevel < 3)
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
            int oneScore, twoScore;
            Debug.Log("Level " + Player.currentLevel);
            oneScore = PlayerPrefs.GetInt("LevelOneScore");
            twoScore = PlayerPrefs.GetInt("LevelTwoScore");

            PlayerPrefs.DeleteAll();

            PlayerPrefs.SetInt("TotalScore", Player.totalScore);

            PlayerPrefs.SetInt("CurrentLevel", Player.currentLevel);

            if (PlayerPrefs.GetInt("CurrentLevel") == 1)
                PlayerPrefs.SetInt("LevelOneScore", Player.totalScore);
            else
                PlayerPrefs.SetInt("LevelOneScore", oneScore);

            if (PlayerPrefs.GetInt("CurrentLevel") == 2)
                PlayerPrefs.SetInt("LevelTwoScore", Player.score + Convert.ToInt32(timeBonus));
            else
                PlayerPrefs.SetInt("LevelTwoScore", twoScore);

            if (PlayerPrefs.GetInt("CurrentLevel") == 3)
                PlayerPrefs.SetInt("LevelThreeScore", Player.score + Convert.ToInt32(timeBonus));
            
            PlayerPrefs.SetInt("ExtraLives", Player.extraLives);

            GameObject.FindObjectOfType<ScreenFader>().speed = 1;

            if (Player.currentLevel < 3) {
                GameObject.FindObjectOfType<ScreenFader>().EndScene(Player.currentLevel + 1);
            }
            else {

                GameObject.FindObjectOfType<ScreenFader>().EndScene(6);

                var scorePath = Path.GetDirectoryName(Application.dataPath);
                scorePath += "/Assets/HiScores.csv";
                var scoreToFile = new string[5];
                var currentDate = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

                if (File.Exists(scorePath))
                {
                    var fs = File.OpenRead(scorePath);
                    var reader = new StreamReader(fs);
                    var scores = new string[5];
                    var scoresValues = new int[5];
                    var dateTimes = new string[5];
                    int pos = 0;

                    while (!reader.EndOfStream && pos < 5)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(';');
                        if (values[0] != "")
                        {
                            scores[pos] = values[0];
                            dateTimes[pos] = values[1];
                        }
                        else
                        {
                            scores[pos] = "0";
                            dateTimes[pos] = "Not yet completed";
                        }
                        pos++;
                    }

                    fs.Close();

                    if (pos < 5)
                    {
                        scores[pos] = Convert.ToString(Player.totalScore);
                        dateTimes[pos] = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                    }
                    else
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            if (scores[i].Equals(""))
                            {
                                scoresValues[i] = 0;
                            }
                            else
                            {
                                scoresValues[i] = Convert.ToInt32(scores[i]);
                            }
                        }

                        if (scoresValues[4] < Player.totalScore)
                        {
                            int tempScore = -1;
                            string tempDate = "";

                            for (int i = 0; i < 5; i++)
                            {
                                if (scoresValues[i] < Player.totalScore)
                                {
                                    tempScore = scoresValues[i];
                                    tempDate = dateTimes[i];
                                    scores[i] = Convert.ToString(Player.totalScore);
                                    dateTimes[i] = currentDate;
                                    Player.totalScore = tempScore;
                                    currentDate = tempDate;
                                }
                            }
                        }

                        for (int i = 0; i < 5; i++)
                        {
                            scoreToFile[i] = scores[i] + ";" + dateTimes[i];
                        }
                    }
                }
                else {
                    File.Create(scorePath);
                    scoreToFile[0] = Convert.ToString(Player.totalScore) + ";" + currentDate;
                }

                File.WriteAllLines(scorePath, scoreToFile);
            }
                
        }
    }
}
