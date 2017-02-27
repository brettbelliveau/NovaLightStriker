using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageCollider : MonoBehaviour {

    public GameObject enemyObject;
    string pointsForKill;

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
            var script = enemyObject.GetComponent<ShadeKnight>();
            if (script == null)
            {
                var script2 = enemyObject.GetComponent<ShadeRanger>();

                if (script2 == null)
                {
                    var script3 = enemyObject.GetComponent<ShadeCrawler>();
                    script3.takingDamage = true;
                    script3.counter = 0;
                    pointsForKill = "50 x " + Player.addScorePoints(50);
                }

                else
                {
                    script2.takingDamage = true;
                    script2.counter = 0;
                    pointsForKill = "100 x " + Player.addScorePoints(100);
                }
            }
            else
            {
                script.takingDamage = true;
                script.counter = 0;
                pointsForKill = "100 x " + Player.addScorePoints(100);
            }

            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            enemyObject.GetComponent<BoxCollider2D>().enabled = false;

        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        //If it's the sword collider from the character
        if (other.gameObject.layer == 15)
        {
            var script = enemyObject.GetComponent<ShadeKnight>();
            if (script == null)
            {
                var script2 = enemyObject.GetComponent<ShadeRanger>();

                if (script2 == null)
                {
                    var script3 = enemyObject.GetComponent<ShadeCrawler>();
                    script3.takingDamage = true;
                    script3.counter = 0;
                }

                else
                {
                    script2.takingDamage = true;
                    script2.counter = 0;
                }
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
