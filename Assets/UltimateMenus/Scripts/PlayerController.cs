using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	/// <summary>
	/// To contact me for any reason, please email me at jadewirestudios@gmail.com. 
	/// </summary>

	public float movementSpeed; // A float which determines how fast our player moves
	public float jumpForce; // A float which determines how much force to add when we jump

	Rigidbody2D rb2d; // A reference to the rigidbody attatched to the current object

	public bool isGrounded; // A bool which determines when our player is touching the ground

	public bool playJumpSound; // A bool which determines wether or not we play a sound when we jump
	public AudioClip jumpSound; // The sound that gets played when we jump

	MenuController_Paused pauseManager; // A reference to the MenuController used for our pause menu


	void Start()
	{
		rb2d = GetComponent<Rigidbody2D> (); // We instantiate the rb2d as a rigidbody attatched to this game object
		pauseManager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<MenuController_Paused> (); // We set the pauseManager as a script found on a GameObject with a tag of "GameManager"
	}

	void Update ()
	{
			if (Input.GetButton ("MoveLeft")) { // If we press the "MoveLeft" button which is setup in the input tab of the settings for your game: 
				transform.position += Vector3.left * movementSpeed * Time.deltaTime; // We move our player to the left by a factor of movementSpeed and deltaTime
			}

			if (Input.GetButton ("MoveRight")) { // If we press the "MoveRight" button, which is also set up in the same manner as the "MoveLeft" button:
				transform.position += Vector3.right * movementSpeed * Time.deltaTime; // We move to the right by a factor of movementSpeed and deltaTime
			}
	

		if (Input.GetButtonDown ("Jump") && isGrounded && !pauseManager.isPaused) { // If we press the "Jump" button, also in the input settings, AND we are grounded, AND we aren't paused:
			rb2d.AddForce (Vector2.up * jumpForce); // We add force upwards to pur player.
			if (playJumpSound) { // And if we want to play a sound:
				AudioSource playerAudio = GetComponent<AudioSource> (); // We create an AudioSource for that sound
				playerAudio.clip = jumpSound; // We set its clip equal to the jumpSound clip
				playerAudio.Play (); // And we play it.
			}

		}			
	}

	void OnCollisionEnter2D(Collision2D other) // When we collide with a 2D object:
	{
		if (other.collider.CompareTag ("GroundObject")) { // And it has a tag of "GroundObject":
			isGrounded = true; // We are touching the ground
		}
	}

	void OnCollisionExit2D(Collision2D other) // But when we leave that object, and until we touch it again:
	{
		if (other.collider.CompareTag ("GroundObject")) {
			isGrounded = false; // We are no longer grounded.
		}
	}
}

