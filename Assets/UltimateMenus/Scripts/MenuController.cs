using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour 
{
	/// <summary>
	/// To contact me for any reason, please email me at jadewirestudios@gmail.com. 
	/// </summary>

	public string canvasIndex; // The string which defines which canvas is being loaded at any given point in time
	public string startingCanvasIndex; // The string which tells the game which canvas to start on

	void Start()
	{
		canvasIndex = startingCanvasIndex; // We set the current canvas index equal to that of the starting canvas index string
	}
}
