using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jumpThrough : MonoBehaviour {
	public string playerName = "Character";
	private GameObject[] allEntities;

	//Find player by name
	void Start () {

	}

	//Check to see if player is under the platform. Collide only if the player is above the platform.
	void FixedUpdate () {
		if (allEntities != null) {
			for (int i = 0; i < allEntities.Length; i++) {
				if (allEntities [i] != null) {
					Physics2D.IgnoreCollision (allEntities [i].GetComponent<BoxCollider2D> (), this.GetComponent<BoxCollider2D> (), this.GetComponent<BoxCollider2D> ().bounds.max.y >= allEntities [i].GetComponent<BoxCollider2D> ().bounds.min.y);
				}
			}
		} else {
			allEntities = GameObject.FindGameObjectsWithTag("Entity");
		}
	}
}
