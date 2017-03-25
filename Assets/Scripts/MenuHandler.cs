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
            if (fade)
                GameObject.FindObjectOfType<ScreenFader>().EndScene(SceneToLoad);
            else
                SceneManager.LoadScene(SceneToLoad);
        }
    }
    
}
