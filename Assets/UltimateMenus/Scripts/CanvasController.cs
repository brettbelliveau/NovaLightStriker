using UnityEngine;
using System.Collections;

public class CanvasController : MonoBehaviour 
{
	/// <summary>
	/// To contact me for any reason, please email me at jadewirestudios@gmail.com. 
	/// </summary>

	MenuController menuControl; // A reference to the "MenuController" script

	public string myIndex; // The string which defines which canvas we are

	Canvas thisCanvas; // A private Canvas which tells our game what this canvas actually is

	void Start()
	{
		menuControl = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<MenuController> (); // We set our menuControl equal to a game object with a tag of GameManager - which holds a component of Canvas Indexer.
		thisCanvas = gameObject.GetComponent<Canvas> (); // We set our thisCanvas variable equal to the canvas attatched to this game object.

		if (myIndex == menuControl.canvasIndex) // If the index that defines our current canvas is the same as the string declared on our MenuController:
		{
			thisCanvas.enabled = true; // We enable our canvas at start
		}

		else // If that's not the case:
		{
			thisCanvas.enabled = false; // We disable our canvas at start.
		}
	}

	void Update() 
	{
		if (myIndex == menuControl.canvasIndex) { // If, at any point in time, our string matches the string that is defined on the MenuController...
			thisCanvas.enabled = true; // We enable our canvas
		} else { // In any other case...
			thisCanvas.enabled = false; // We disable our canvas.
		}
	}
}
