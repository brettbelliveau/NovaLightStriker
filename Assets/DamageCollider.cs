using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //Ignore character layer collisions
        Physics2D.IgnoreLayerCollision(9, 9, true);
    }
    
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        //If collider belongs to an enemy, and not
        //taking damage or spawning
        if (other.gameObject.layer == 14 && !Player.takingDamage && !Player.spawningBool)
        {
            Player.takingDamage = true;
            Player.counter = 0;
            Debug.Log("ENTERING SWORD WOOHOO");
        }

        //Else do nothing
    }

    void OnTriggerStay2D(Collider2D other)
    { 
        
        //If collider belongs to an enemy, and not
        //taking damage or spawning
        if (other.gameObject.layer == 14 && !Player.takingDamage && !Player.spawningBool)
        {
            Player.takingDamage = true;
            Player.counter = 0;
            Debug.Log("ENTERING SWORD WOOHOO");
        }

        //Else do nothing

    }
}
