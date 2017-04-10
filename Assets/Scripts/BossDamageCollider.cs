﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDamageCollider : MonoBehaviour {

    public GameObject enemyObject;
    int pointsForKill;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        //If it's the sword collider from the character
        if (other.gameObject.layer == 15)
        {
			var script = enemyObject.GetComponent<ShadeMageBoss> ();
			if (script == null) {
				var script2 = enemyObject.GetComponent<PurpleSlime> ();
				if (script2 != null) {
					gameObject.GetComponent<BoxCollider2D>().enabled = false;
					enemyObject.GetComponent<BoxCollider2D>().enabled = false;
					script2.sendDamage (10);
					script2.counter = 0;
				}
			} else {
				script.sendDamage(10);
			}
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        //If it's the sword collider from the character
        //Add instance for third enemy type
        if (other.gameObject.layer == 15)
        {
			var script = enemyObject.GetComponent<ShadeMageBoss> ();
			if (script != null) {
				script.sendDamage (10);
			}
        }
    }
}
