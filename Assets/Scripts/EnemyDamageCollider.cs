using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDamageCollider : MonoBehaviour {

    public GameObject enemyObject, scoreText;
    private GameObject tempText;
    private int counter = 0;
    private string pointsForKill;
    private int multiplier;
    private Vector3 textLocation;
    private bool triggered = false;

	// Use this for initialization
	void Start () {
        multiplier = 0;
	}
	
	// Update is called once per frame
	void Update () {

        if (Time.timeScale != 1)
            return;

        if (tempText != null)
        {
            counter = (counter + 1) % 60;
            tempText.transform.localPosition = new Vector3
                (textLocation.x, tempText.transform.localPosition.y + 0.005f, textLocation.z);

            if (counter == 0)
            {
                tempText.GetComponent<TextMesh>().text = "";
                Destroy(tempText);
            }
        }
	}

    void OnTriggerEnter2D (Collider2D other)
    {
        //If it's the sword collider from the character
        if (other.gameObject.layer == 15 && !triggered)
        {
            triggered = true;
            var script = enemyObject.GetComponent<ShadeKnight>();
            if (script == null)
            {
                var script2 = enemyObject.GetComponent<ShadeRanger>();

                if (script2 == null)
                {
                    var script3 = enemyObject.GetComponent<ShadeCrawler>();
                    gameObject.GetComponent<BoxCollider2D>().enabled = false;
                    enemyObject.GetComponent<BoxCollider2D>().enabled = false;
                    script3.takingDamage = true;
                    script3.counter = 0;
                    pointsForKill = "50";
                    Player.addScorePoints(50);
                    textLocation = new Vector3(0.5f, 0, 0);
                }

                else
                {
                    gameObject.GetComponent<BoxCollider2D>().enabled = false;
                    enemyObject.GetComponent<BoxCollider2D>().enabled = false;
                    script2.takingDamage = true;
                    script2.counter = 0;
                    pointsForKill = "100";
                    Player.addScorePoints(100);
                    textLocation = new Vector3(-0.1f, 0, 0);
                }
            }
            else
            {
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                enemyObject.GetComponent<BoxCollider2D>().enabled = false;
                script.takingDamage = true;
                script.counter = 0;
                pointsForKill = "100";
                Player.addScorePoints(100);
                textLocation = new Vector3(-0.1f, 0, 0);
            }

            tempText = spawnTextAtLocation(textLocation);

        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        //If it's the sword collider from the character
        if (other.gameObject.layer == 15 && !triggered)
        {
            triggered = true;
            var script = enemyObject.GetComponent<ShadeKnight>();
            if (script == null)
            {
                var script2 = enemyObject.GetComponent<ShadeRanger>();

                if (script2 == null)
                {
                    var script3 = enemyObject.GetComponent<ShadeCrawler>();
                    script3.takingDamage = true;
                    script3.counter = 0;
                    pointsForKill = "50";
                    Player.addScorePoints(50);
                    textLocation = new Vector3(0.5f, 0, 0);
                    gameObject.GetComponent<BoxCollider2D>().enabled = false;
                    enemyObject.GetComponent<BoxCollider2D>().enabled = false;
                }

                else
                {
                    script2.takingDamage = true;
                    script2.counter = 0;
                    pointsForKill = "100";
                    Player.addScorePoints(100);
                    textLocation = new Vector3(-0.1f, 0, 0);
                    gameObject.GetComponent<BoxCollider2D>().enabled = false;
                    enemyObject.GetComponent<BoxCollider2D>().enabled = false;
                }
            }
            else
            {
                script.takingDamage = true;
                script.counter = 0;
                pointsForKill = "100";
                Player.addScorePoints(100);
                textLocation = new Vector3(-0.1f, 0, 0);
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                enemyObject.GetComponent<BoxCollider2D>().enabled = false;
            }
            
            tempText = spawnTextAtLocation(textLocation);

        }
    }

    private GameObject spawnTextAtLocation(Vector3 location)
    {
        var popUpText = Instantiate(scoreText, location, Quaternion.identity) as GameObject;

        popUpText.transform.parent = enemyObject.transform;
        popUpText.transform.localPosition = new Vector3(location.x, location.y+1f, -3f);

        //If player not in hyper mode yet (or recently turned), do x
        if (Player.multiplier > 2 && Player.multiplier < 8 || Player.multiplier == 8 && Time.time * 1000 <= Player.hyperActiveTime + 500)
            pointsForKill = pointsForKill + " x " + (Player.multiplier - 1);

        else if (Player.multiplier == 2) { } //List only points

        //If in hyper mode..
        else
            pointsForKill = pointsForKill + " x " + Player.multiplier;

        popUpText.GetComponent<TextMesh>().text = pointsForKill;
        
        popUpText.SetActive(true);
        
        return popUpText;
    }
}
