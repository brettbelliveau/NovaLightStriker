using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jumpThrough : MonoBehaviour {
	public string playerName = "Character";
	private GameObject player;

	//Find player by name
	void Start () {

	}

	//Check to see if player is under the platform. Collide only if the player is above the platform.
	void FixedUpdate () {
		if (player != null) {
			Physics2D.IgnoreCollision(player.GetComponent<BoxCollider2D>(), this.GetComponent<BoxCollider2D>(), this.GetComponent<BoxCollider2D> ().bounds.max.y >= player.GetComponent<BoxCollider2D> ().bounds.min.y);
		} else {
			player = GameObject.FindWithTag("Entity");
		}
	}
}
