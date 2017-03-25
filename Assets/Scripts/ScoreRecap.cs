using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreRecap : MonoBehaviour {

    public GameObject BG, Complete, Score, TimeBonus, Total, ScoreVal, TimeVal, TotalVal;
    private int counter;
    public bool run;
    private int timeBonus;
    
	// Use this for initialization
	void Start () {
        counter = 0;
        ScoreVal.GetComponent<Text>().text = Convert.ToString(Player.score);
        TimeVal.GetComponent<Text>().text = Convert.ToString(timeBonus); //TODO: Calc time bonus based on time compl and level
        TotalVal.GetComponent<Text>().text = Convert.ToString(Player.score + timeBonus);
        run = false;
	}
	
	// Update is called once per frame
	void Update () {

        if (run)
            counter++;

        if (counter == 1)
            Complete.SetActive(true);

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

        else if (counter == 500) { }
            //Load next level here
    }
}
