using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    private Color normal;
    private Color light;
    private string normHex = "#21FFEFFF";
    private string lightHex = "#B8FFFAFF";

    public static bool alternate;
    public static bool backToNormal;
    private int counter = 0;

    public GameObject fill;

    // Use this for initialization
    void Start()
    {
        ColorUtility.TryParseHtmlString(normHex, out normal);
        ColorUtility.TryParseHtmlString(lightHex, out light);
    }

    // Update is called once per frame
    void Update()
    {
        if (alternate)
        {
            counter = (counter + 1) % 20;
            if (counter == 0)
            {
                fill.GetComponent<Image>().color = light;
            }
            else if (counter == 10)
            {
                fill.GetComponent<Image>().color = normal;
            }
        }

        else if (backToNormal)
        {
            fill.GetComponent<Image>().color = normal;
            backToNormal = false;
            counter = 0;
        }
    }

}
