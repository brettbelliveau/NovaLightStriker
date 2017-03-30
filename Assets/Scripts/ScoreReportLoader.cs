using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreReportLoader : MonoBehaviour {

    public GameObject scoreList;

    private int[] scores;

	// Use this for initialization
	void Start () {

        //TODO: Load these arrays from text file
        //If no text flie, intialize one with this data
        scores = new int[4];
        scores[0] = PlayerPrefs.GetInt("LevelOneScore");
        scores[1] = PlayerPrefs.GetInt("LevelTwoScore");
        scores[2] = PlayerPrefs.GetInt("LevelThreeScore");
        scores[3] = scores[2] + scores[1] + scores[0];
        scoreList.GetComponent<Text>().text = buildList(scores);
    }
	
    private string buildList(int[] list)
    {
        string str = "";
        for (int i = 0; i < list.Length; i++)
        {
            str += list[i];

            if (i < list.Length - 1)
                str += "\n" + "\n";
            if (i == list.Length - 2)
                str += "\n";
        }

        return str;
    }

}
