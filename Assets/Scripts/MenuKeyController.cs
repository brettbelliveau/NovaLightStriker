using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuKeyController : MonoBehaviour {

    public GameObject[] buttons;

    private Color unSelected;
    private Color selected;

    private static int cursor;

	// Use this for initialization
	void Start () {
        unSelected = buttons[0].GetComponent<ButtonController>().regularColor;
        selected = buttons[0].GetComponent<ButtonController>().highlightedColor;

        cursor = PlayerPrefs.GetInt("CurrentLevel") == 0 ? PlayerPrefs.GetInt("LastCursorPosition") : 0;
    }
	
	// Update is called once per frame
	void Update () {

        //Select button at cursor
        if (Input.GetButtonDown("Submit"))
        {
            buttons[cursor].GetComponent<MenuHandler>().enabled = true;
            if (PlayerPrefs.GetInt("CurrentLevel") == 0)
            {
                PlayerPrefs.SetInt("LastCursorPosition", cursor);
            }
        }

        //Move cursor up
        if (Input.GetButtonDown("UP"))
        {
            if (cursor - 1 >= 0)
            {
                buttons[cursor--].GetComponentInChildren<Text>().color = unSelected;
            }
        }
        //Move cursor down
        else if (Input.GetButtonDown("DOWN"))
        {
            if (cursor + 1 < buttons.Length)
            {
                buttons[cursor++].GetComponentInChildren<Text>().color = unSelected;
            }
        }
        
        buttons[cursor].GetComponentInChildren<Text>().color = selected;
    }    
}
