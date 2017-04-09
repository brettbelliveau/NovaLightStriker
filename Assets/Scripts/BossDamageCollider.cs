using System.Collections;
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
            if (Player.currentLevel == 1)
            {
                enemyObject.GetComponent<ShadeMageBoss>().sendDamage(10);
            }
           //  else if (Player.currentLevel == 2)
           // {
           //     enemyObject.GetComponent<PurpleSlime>().sendDamage(10);
           // }
            else
            {
                enemyObject.GetComponent<SkeletonBoss>().sendDamage(10);
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        //If it's the sword collider from the character
        //Add instance for third enemy type
        if (other.gameObject.layer == 15)
        {
            if (Player.currentLevel == 1)
            {
                enemyObject.GetComponent<ShadeMageBoss>().sendDamage(10);
            }
            //  else if (Player.currentLevel == 2)
            // {
            //     enemyObject.GetComponent<PurpleSlime>().sendDamage(10);
            // }
            else
            {
                enemyObject.GetComponent<SkeletonBoss>().sendDamage(10);
            }
        }
    }
}
