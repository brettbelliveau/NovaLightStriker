using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreLoader : MonoBehaviour {

    public GameObject scoreList;
    public GameObject dateTimeList;

    private string[] scores, dateTimes;

	// Use this for initialization
	void Start () {

        //TODO: Load these arrays from text file
        //If no text flie, intialize one with this data
        scores = new string[] { "50000", "45000", "40000", "35000", "30000" };
        dateTimes = new string[] { "1/01/2000 12:00:00 AM", "1/01/2000 12:00:00 AM",
            "1/01/2000 12:00:00 AM", "1/01/2000 12:00:00 AM", "1/01/2000 12:00:00 AM" };

        scoreList.GetComponent<Text>().text = buildList(scores);
        dateTimeList.GetComponent<Text>().text = buildList(dateTimes);
    }
	
    private string buildList(string[] list)
    {
        string str = "";
        for (int i = 0; i < list.Length; i++)
        {
            str += list[i];

            if (i < list.Length - 1)
                str += "\n";
        }

        return str;
    }

}
