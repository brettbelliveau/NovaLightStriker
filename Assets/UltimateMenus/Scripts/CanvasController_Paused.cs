using UnityEngine;
using System.Collections;

public class CanvasController_Paused : MonoBehaviour 
{

	/// <summary>
	/// To contact me for any reason, please email me at jadewirestudios@gmail.com. 
	/// </summary>

	MenuController_Paused pauseControl; // A reference to the MenuController for our pause menu
	public string myIndex; // A string which defines the index of this currently loaded canvas
	Canvas myCanvas; // A canvas which defines what the canvas is on the object this script is attached to


	void Start()
	{
		myCanvas = gameObject.GetComponent<Canvas> (); // We set our canvas
		pauseControl = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<MenuController_Paused> (); // We define the pauseControl variable
	}

	void Update()
	{

		if (pauseControl.isPaused) { // If we are paused,
			if (pauseControl.canvasIndex == myIndex) { // AND if the canvas index on the MenuController_Paused is the same as ours:
				myCanvas.enabled = true; // We render the canvas and all of its components.
			}

			if (pauseControl.canvasIndex != myIndex) { // But if we are paused, and the indexes aren't the same...
				myCanvas.enabled = false; // We do NOT render this canvas.
			}
		}

		if (!pauseControl.isPaused) { // And, if at any point in time our game is not paused, we do NOT render our canvas.
			myCanvas.enabled = false;
		}
	}
}
