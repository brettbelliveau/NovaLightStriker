﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour
{
    public Image fadeImage;
    public float speed;
    private bool startScene = true;
    
    public bool fadeIn;

    void Awake()
    {
        fadeImage.rectTransform.localScale = new Vector2(Screen.width, Screen.height);
    }

    void Update()
    {
        if (startScene)
        {
            if (PlayerPrefs.GetInt("CurrentLevel") == 0 && (PlayerPrefs.GetInt("PreviousLevel") < 4 || PlayerPrefs.GetInt("PreviousLevel") > 5))
            {
                fadeIn = true;
                PlayerPrefs.SetInt("LastCursorPosition", 0);
            }

            if (fadeIn)
                StartScene();
            else
            {
                fadeImage.color = Color.clear;
                fadeImage.enabled = startScene = false;
            }
        }
    }

    void StartScene()
    {
        FadeToEmpty();

        if (fadeImage.color.a <= 0.05f)
        {
            // ... set the colour to clear and disable the RawImage.
            fadeImage.color = Color.clear;
            fadeImage.enabled = false;

            // The scene is no longer starting.
            startScene = false;
        }
    }

    void FadeToEmpty()
    {
        fadeImage.color = Color.Lerp(fadeImage.color, Color.clear, speed * Time.deltaTime);
    }


    void FadeToBlack()
    {
        fadeImage.color = Color.Lerp(fadeImage.color, Color.black, speed * Time.deltaTime);
    }
    
    public IEnumerator EndSceneRoutine(int SceneNumber)
    {
        fadeImage.enabled = true;
        while (true)
        {
            FadeToBlack();
            if (fadeImage.color.a >= 0.96f)
            {
                if (SceneNumber > -1)
                {
                    PlayerPrefs.SetInt("PreviousLevel", PlayerPrefs.GetInt("CurrentLevel"));
                    PlayerPrefs.SetInt("CurrentLevel", SceneNumber);
                    SceneManager.LoadScene(SceneNumber);
                }
                else
                    Application.Quit();
                yield break;
            }
            else
            {
                yield return null;
            }
        }
    }

    public void EndScene(int SceneNumber)
    {
        startScene = false;
        StartCoroutine("EndSceneRoutine", SceneNumber);
    }
}