using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

public class ButtonController_Paused : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
	/// <summary>
	/// To contact me for any reason, please email me at jadewirestudios@gmail.com. 
	/// </summary>

	//
	/// <summary>
	/// Also, this script is VERY similiar in relation to the ButtonController for the main menu. Not that much is different, just functions which act in different ways.
	/// </summary>

	MenuController_Paused pauseControl;

	Text textForButton;

	public bool playHoverSound;
	public AudioClip hoverSound;

	public bool playClickSound;
	public AudioClip clickSound;

	public bool modifyFontColor;
	public Color startingTextColor;
	public Color textColorHighlighted;

	public bool modifyFontSize;
	public int startingFontSize;
	public int fontSizeHighlighted;

	AudioSource thisAudioSource;

	void Start()
	{
		textForButton = gameObject.GetComponentInChildren<Text> ();
		pauseControl = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<MenuController_Paused> ();
		thisAudioSource = gameObject.GetComponent<AudioSource> ();
		thisAudioSource.playOnAwake = false;

		if (modifyFontSize) {
			textForButton.fontSize = startingFontSize;
		}

		if (modifyFontColor) {
			textForButton.color = startingTextColor;
		}
	}

	public void UnPause()
	{
		pauseControl.isPaused = false;
		pauseControl.CheckPause ();
	}

	public void LoadCanvas(string CanvasIndex)
	{
		pauseControl.canvasIndex = CanvasIndex;
	}

	public void LoadURL(string URL)
	{
		Application.OpenURL (URL);
	}

	public void LoadScene(string MenuName)
	{
		SceneManager.LoadScene (MenuName);
	}

	public void Quit()
	{
		Application.Quit ();
	}

	public void OnPointerEnter(PointerEventData eventdata)
	{
		if (modifyFontColor) {
			textForButton.color = textColorHighlighted;
		}

		if (modifyFontSize) {
			textForButton.fontSize = fontSizeHighlighted;
		}

		if (playHoverSound) {
			thisAudioSource.clip = hoverSound;
			thisAudioSource.Play ();
		}
	}

	public void OnPointerExit(PointerEventData eventdata)
	{
		if (modifyFontColor) {
			textForButton.color = startingTextColor;
		}

		if (modifyFontSize) {
			textForButton.fontSize = startingFontSize;
		}
	}

	public void OnPointerDown(PointerEventData eventdata)
	{
		if (playClickSound) {
			thisAudioSource.clip = clickSound;
			thisAudioSource.Play ();
		}
	}
}
