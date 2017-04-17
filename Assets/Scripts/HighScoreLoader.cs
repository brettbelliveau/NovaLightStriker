using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreLoader : MonoBehaviour {

    public GameObject scoreList;
    public GameObject dateTimeList;

    private string[] scores, dateTimes;

	// Use this for initialization
	void Start () {
        var scorePath = Path.GetDirectoryName(Application.dataPath);
        scorePath += "/Assets/HiScores.csv";

        if (File.Exists(scorePath)) {
            var fs = File.OpenRead(scorePath);
            var reader = new StreamReader(fs);
            scores = new string[5];
            dateTimes = new string[5];
            int pos = 0;

            while(!reader.EndOfStream && pos < 5)
            {
                Debug.Log("Position: " + pos);
                var line = reader.ReadLine();
                Debug.Log("Line is: " + line);
                var values = line.Split(';');
                if (values[0] != "")
                {
                    Debug.Log("Score is: " + values[0]);
                    scores[pos] = values[0];
                    Debug.Log("Date is: " + values[1]);
                    dateTimes[pos] = values[1];
                }
                else
                {
                    scores[pos] = "0";
                    dateTimes[pos] = "Not yet completed.";
                }
                pos++;
            }

            if(pos < 5)
            {
                while(pos < 5)
                {
                    scores[pos] = "0";
                    dateTimes[pos] = "Not yet completed.";
                    pos++;
                }
            }
        }
        else
        {
            File.Create(scorePath);
            //If no text flie, intialize one with this data
            scores = new string[] { "50000", "45000", "40000", "35000", "30000" };
            dateTimes = new string[] { "1/01/2000 12:00:00 AM", "1/01/2000 12:00:00 AM",
            "1/01/2000 12:00:00 AM", "1/01/2000 12:00:00 AM", "1/01/2000 12:00:00 AM" };
        }


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
