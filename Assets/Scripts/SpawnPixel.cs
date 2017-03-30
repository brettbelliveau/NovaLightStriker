using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPixel : MonoBehaviour {

    public Rigidbody2D body;
    public GameObject snow;

    private int counter = 0;
    
	// Use this for initialization
	void Start () {
        body = gameObject.GetComponent<Rigidbody2D>();
        
        //Ignore spawn pixels (layer 10) collision with characters (layer 9)
        Physics2D.IgnoreLayerCollision(9, 10, true);
        //Ignore all VFX collisions (snow, spawn pixels, etc)
        Physics2D.IgnoreLayerCollision(10, 10, true);
    }
	
	// Update is called once per frame
	void Update () {

        if (Time.timeScale != 1)
            return;

        //If falling pixels
        if (counter++ > 5 && body.velocity.y < 0.02 && snow.transform.position.y <= SpawnInOutPixels.deleteSpot)
        {
            SpawnInOutPixels.deleteSpot += 0.005f;
            Destroy(snow);
            Destroy(this);
        }

        //Else if snow rising too long, delete
        else if (counter > 200)
        {
            Destroy(snow);
            Destroy(this);
        }
    }
}
