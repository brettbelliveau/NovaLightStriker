using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageCollider : MonoBehaviour {

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
        //Add instance for third enemy type
        if (other.gameObject.layer == 15)
        {
            var script = enemyObject.GetComponent<ShadeKnight>();
            if (script == null)
            {
                var newScript = enemyObject.GetComponent<ShadeRanger>();
                newScript.takingDamage = true;
                newScript.counter = 0;
            }
            else
            {
                script.takingDamage = true;
                script.counter = 0;
            }

            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            enemyObject.GetComponent<BoxCollider2D>().enabled = false;

        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        //If it's the sword collider from the character
        //Add instance for third enemy type
        if (other.gameObject.layer == 15)
        {
            var script = enemyObject.GetComponent<ShadeKnight>();
            if (script == null)
            {
                var newScript = enemyObject.GetComponent<ShadeRanger>();
                newScript.takingDamage = true;
                newScript.counter = 0;
            }
            else
            {
                script.takingDamage = true;
                script.counter = 0;
            }

            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            enemyObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
