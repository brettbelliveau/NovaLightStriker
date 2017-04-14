using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour {

    private static AudioScript instance = null;
    private bool control = false;
    public static bool fadeOutSound = false;
    public static AudioScript Instance
    {
        get { return instance; }
    }
    void Awake()
    {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            return;
        } else {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
    // Use this for initialization
    void Start () {
        fadeOutSound = false;
    }

    // Update is called once per frame
    void Update() {

        if (PlayerPrefs.GetInt("CurrentLevel") < 1 || PlayerPrefs.GetInt("CurrentLevel") > 3)
        {
            if (!gameObject.GetComponent<AudioSource>().isPlaying)
            {
                gameObject.GetComponent<AudioSource>().Play();
            }

            if (gameObject.GetComponent<AudioSource>().volume < .12f && !fadeOutSound)
            {
                gameObject.GetComponent<AudioSource>().volume += 0.001f;
            }

            else if (fadeOutSound)
            {
                gameObject.GetComponent<AudioSource>().volume -= 0.002f;
            }
        }
    }
}
