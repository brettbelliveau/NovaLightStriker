using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    
    private float speed = 5.5f;
    public bool onGround;
    private int counter = 0;

    private Rigidbody2D body;
    private SpriteRenderer spriteRender;
    private BoxCollider2D collider;

    public Sprite standing, runningOne, runningTwo, runningThree, runningFour, runningFive,
                  runningSix, runningSeven, runningEight, runningNine, jumping;

	// Use this for initialization
	void Start () {
        body = gameObject.GetComponent<Rigidbody2D>();
        collider = gameObject.GetComponent<BoxCollider2D>();
        spriteRender = gameObject.GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {

        //Check if back on ground after jump/fall
        if (!onGround && Mathf.Abs(body.velocity.y) < 0.02)
            onGround = true;
        
        //Turn off onGround when falling from ledge
        if (onGround)
            onGround = Mathf.Abs(body.velocity.y) < 0.02;

        if (Input.GetButtonDown("Jump") && onGround)
        {
            body.AddForce(Vector2.up * 170);
            onGround = false;
        }

        /* Sprite Section */

        //Standing sprite
        if (body.velocity.x == 0 && Mathf.Abs(body.velocity.y) < 0.02)
        {
            spriteRender.sprite = standing;
        }

        //Running sprite
        else if (body.velocity.x != 0 && Mathf.Abs(body.velocity.y) < 0.02)
        {
            counter = (counter + 1) % 18;
            Debug.Log(counter);
            if (counter >= 0 && counter < 2)
                spriteRender.sprite = runningOne;
            else if (counter >= 2 && counter < 4)
                spriteRender.sprite = runningTwo;
            else if (counter >= 4 && counter < 6)
                spriteRender.sprite = runningThree;
            else if (counter >= 6 && counter < 8)
                spriteRender.sprite = runningFour;
            else if (counter >= 8 && counter < 10)
                spriteRender.sprite = runningFive;
            else if (counter >= 10 && counter < 12)
                spriteRender.sprite = runningSix;
            else if (counter >= 12 && counter < 14)
                spriteRender.sprite = runningSeven;
            else if (counter >= 14 && counter < 16)
                spriteRender.sprite = runningEight;
            else if (counter >= 16 && counter < 18)
                spriteRender.sprite = runningNine;

        }

        //Jumping sprite
        else if (body.velocity.y != 0)
        {
            spriteRender.sprite = jumping;
        }
    
    }

    void FixedUpdate () {

        //Obtain left/right input
        float h = Input.GetAxisRaw("Horizontal");
        body.velocity = new Vector2(speed * h, body.velocity.y);
        
        //Turn off collider when moving upward
        collider.enabled = body.velocity.y <= 0.02;

        //Left orientation for sprite if going left, or if standing still and already left
        spriteRender.flipX = (body.velocity.x < 0) || (spriteRender.flipX && body.velocity.x == 0);
    }
}
