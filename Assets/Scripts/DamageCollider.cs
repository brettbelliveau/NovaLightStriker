using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour {

    public bool isLeft;
    private int projectileDamage = 20;
    private int physicalDamage = 10;

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
        if (other.gameObject.layer == 14 && !Player.takingDamage && !Player.spawningBool && !Player.invincibleFrames && !Player.bossDefeated)
        {
            Player.takingDamage = true;
            Player.counter = 0;
            Player.hitFromLeft.Add(isLeft);
            Player.lastDamageTaken.Add(physicalDamage);
        }

        //If collider belongs to an enemy projectile, and not
        //taking damage or spawning
        else if (other.gameObject.layer == 12 && !Player.takingDamage && !Player.spawningBool && !Player.invincibleFrames && !Player.bossDefeated)
        {
            Player.takingDamage = true;
            Player.counter = 0;
            Player.hitFromLeft.Add(isLeft);
            Player.lastDamageTaken.Add(projectileDamage);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {

        //If collider belongs to an enemy, and not
        //taking damage or spawning
        if (other.gameObject.layer == 14 && !Player.takingDamage && !Player.spawningBool && !Player.invincibleFrames && !Player.bossDefeated)
        {
            Player.takingDamage = true;
            Player.counter = 0;
            Player.hitFromLeft.Add(isLeft);
            Player.lastDamageTaken.Add(physicalDamage);
        }

        //If collider belongs to an enemy projectile, and not
        //taking damage or spawning
        else if (other.gameObject.layer == 12 && !Player.takingDamage && !Player.spawningBool && !Player.invincibleFrames && !Player.bossDefeated)
        {
            Player.takingDamage = true;
            Player.counter = 0;
            Player.hitFromLeft.Add(isLeft);
            Player.lastDamageTaken.Add(projectileDamage);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {

        //If collider belongs to an enemy, and not
        //taking damage or spawning
        if (other.gameObject.layer == 14 && !Player.takingDamage && !Player.spawningBool && !Player.invincibleFrames && !Player.bossDefeated)
        {
            Player.takingDamage = true;
            Player.counter = 0;
            Player.hitFromLeft.Add(isLeft);
            Player.lastDamageTaken.Add(physicalDamage);
        }

        //If collider belongs to an enemy projectile, and not
        //taking damage or spawning
        else if (other.gameObject.layer == 12 && !Player.takingDamage && !Player.spawningBool && !Player.invincibleFrames && !Player.bossDefeated)
        {
            Player.takingDamage = true;
            Player.counter = 0;
            Player.hitFromLeft.Add(isLeft);
            Player.lastDamageTaken.Add(projectileDamage);
        }
    }
        //Misc Notes:
            //By using two of this script we can determine if the character was hit from 
            //right or left, and act accordingly

            //Also, if we add the isLeft bool to a list, even if both right and left are triggered
            //We can check index 0 to see which one came first
            //Pretty good for a lazy solution 8)
        
}
