using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuKeyController : MonoBehaviour {

    public GameObject[] buttons;
    public GameObject PauseMenuItems;

    private bool active;
    private Color unSelected;
    private Color selected;

    private int cursor;

	// Use this for initialization
	void Start () {
        unSelected = buttons[0].GetComponent<ButtonController>().regularColor;
        selected = buttons[0].GetComponent<ButtonController>().highlightedColor;

        cursor = 0;
        PauseMenuItems.SetActive(false);
        active = false;
    }
	
	// Update is called once per frame
	void Update () {

        if (active)
        {
            buttons[cursor].GetComponentInChildren<Text>().color = selected;
            //Select button at cursor
            if (Input.GetButtonDown("Submit"))
            {
                //Resume
                if (cursor == 0)
                {
                    active = false;
                    PauseMenuItems.SetActive(false);
                    Time.timeScale = 1;
                    Player.stopMovement = false;
                }

                //Quit
                else
                {
                    Time.timeScale = 1;
                    int current = PlayerPrefs.GetInt("CurrentLevel");
                    PlayerPrefs.DeleteAll();
                    PlayerPrefs.SetInt("CurrentLevel", current);
                    GameObject.FindObjectOfType<ScreenFader>().EndScene(0);
                }
            }

            //Move cursor up
            if (Input.GetButtonDown("UP"))
            {
                if (cursor - 1 >= 0)
                {
                    buttons[cursor--].GetComponentInChildren<Text>().color = unSelected;
                    buttons[cursor].GetComponentInChildren<Text>().color = selected;
                }
            }
            //Move cursor down
            else if (Input.GetButtonDown("DOWN"))
            {
                if (cursor + 1 < buttons.Length)
                {
                    buttons[cursor++].GetComponentInChildren<Text>().color = unSelected;
                    buttons[cursor].GetComponentInChildren<Text>().color = selected;
                }
            }
            
            else if (Input.GetButtonDown("Cancel"))
            {
                active = false;
                PauseMenuItems.SetActive(false);
                Time.timeScale = 1;
                Player.stopMovement = false;
            }
        }
        
        else //pause menu inactive
        {
            if (Input.GetButtonDown("Cancel") && !Player.bossDefeated)
            {
                active = true;
                PauseMenuItems.SetActive(true);
                Time.timeScale = 0;
                Player.stopMovement = true;
            }
        }
        
    }    
}
