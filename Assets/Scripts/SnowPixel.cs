using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowPixel : MonoBehaviour {

    public Rigidbody2D body;
    public GameObject snow;

    private int counter = 0;
    
	// Use this for initialization
	void Start () {
        body = gameObject.GetComponent<Rigidbody2D>();
        
        //Ignore snow (layer 10) collision with characters (layer 9)
        Physics2D.IgnoreLayerCollision(9, 10, true);
        //Ignore platforms where there is cover overhead (layer 13)
        Physics2D.IgnoreLayerCollision(13, 10, true);
    }
	
	// Update is called once per frame
	void Update () {

        if (Time.timeScale != 1)
            return;

        //If snow has had a chance to start falling, delete on ground hit
        if (counter++ > 5 && body.velocity.y > -0.02)
        {
            Destroy(snow);
            Destroy(this);
        }

        //Else if snow falling too long, delete
        else if (counter > 300)
        {
            Destroy(snow);
            Destroy(this);
        }
    }
}
