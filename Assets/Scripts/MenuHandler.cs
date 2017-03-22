using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHandler : MonoBehaviour {

    int counter = 0;
    
	// Update is called once per frame
	void Update () {
        if (counter++ == 0)
        {
            GameObject.FindObjectOfType<ScreenFader>().EndScene(1);
        }
    }
}
