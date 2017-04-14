using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour {

    public int SceneToLoad;
    public bool fade;

    //Here are the scene numbers for reference:
    //
    //  Number  Scene
    //  0       Main Menu
    //  1       Level One
    //  2       Level Two
    //  3       Level Three
    //  4       About
    //  5       High Scores
    //  6       End Game Recap

    //In case this component is enabled in Unity
    void Awake()
    {
        enabled = false;
    }    

    void Start()
    {
        if (enabled)
        {
            //If starting game
            if (PlayerPrefs.GetInt("CurrentLevel") == 0 && SceneToLoad == 1)
            {
                PlayerPrefs.DeleteAll();
                PlayerPrefs.SetInt("CurrentLevel", 0);
            }
            if (PlayerPrefs.GetInt("CurrentLevel") > 0 && PlayerPrefs.GetInt("CurrentLevel") < 4 && SceneToLoad == 0)
            {
                Player.fadeOutSound = true;
            }

            if (fade)
                GameObject.FindObjectOfType<ScreenFader>().EndScene(SceneToLoad);
            else
            {
                PlayerPrefs.SetInt("PreviousLevel", PlayerPrefs.GetInt("CurrentLevel"));
                PlayerPrefs.SetInt("CurrentLevel", SceneToLoad);
                SceneManager.LoadScene(SceneToLoad);
            }
        }
    }
    
}
