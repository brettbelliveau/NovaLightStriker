using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaDeath : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D other)
	{

		if (other.gameObject.layer == 9)
		{
			Player.takingDamage = true;
			Player.counter = 0;
			Player.hitFromLeft.Add(false);
			Player.lastDamageTaken.Add(100);
		}

	}

	void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.layer == 9)
		{
			Player.takingDamage = true;
			Player.counter = 0;
			Player.hitFromLeft.Add(false);
			Player.lastDamageTaken.Add(100);
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.layer == 9)
		{
			Player.takingDamage = true;
			Player.counter = 0;
			Player.hitFromLeft.Add(false);
			Player.lastDamageTaken.Add(100);
		}
	}
}
