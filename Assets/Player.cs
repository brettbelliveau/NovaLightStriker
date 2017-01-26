using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    
    public float speed = 5.5f;
    public bool onGround;
    public static int counter = 0;
    public int runAnimationSpeed = 3;
    public static int attackAnimationSpeed = 2;

    private Rigidbody2D body;
    private SpriteRenderer spriteRender;
    private BoxCollider2D collider;

    public Sprite standing, jumping;
    public Sprite[] running;
    public Sprite[] attacking;

    private bool attackFreeze;

    // Use this for initialization
    void Start () {
        body = gameObject.GetComponent<Rigidbody2D>();
        collider = gameObject.GetComponent<BoxCollider2D>();
        spriteRender = gameObject.GetComponent<SpriteRenderer>();
        attackFreeze = false;
	}
	
	// Update is called once per frame
	void Update () {

        //Check if back on ground after jump/fall
        if (!onGround && Mathf.Abs(body.velocity.y) < 0.02)
            onGround = true;
        
        //Turn off onGround when falling from ledge
        if (onGround)
            onGround = Mathf.Abs(body.velocity.y) < 0.02;

        if (Input.GetButtonDown("Jump") && onGround && !attackFreeze)
        {
            body.AddForce(Vector2.up * 170);
            onGround = false;
        }

        if (Input.GetButtonDown("Fire1") && !attackFreeze)
        {
            counter = 0;
            attackFreeze = true;
            PixelGenerator.attacking = true;
        }

        /* Sprite Section */

        //Standing sprite
        if (!attackFreeze && body.velocity.x == 0 && Mathf.Abs(body.velocity.y) < 0.02)
        {
            spriteRender.sprite = standing;
        }

        //Running sprite
        else if (!attackFreeze && body.velocity.x != 0 && Mathf.Abs(body.velocity.y) < 0.02)
        {
            counter = (counter + 1) % (running.Length*runAnimationSpeed);
            spriteRender.sprite = running[counter/runAnimationSpeed];
        }

        //Attacking sprite
        else if (attackFreeze)
        {
            counter = (counter + 1) % (attacking.Length * attackAnimationSpeed);
            spriteRender.sprite = attacking[counter / attackAnimationSpeed];
            //Done attacking
            if (counter == 0)
                attackFreeze = PixelGenerator.attacking = false;
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
        //TODO: Change this to layering system
        collider.enabled = body.velocity.y <= 0.02;

        //Left orientation for sprite if going left, or if standing still and already left
        spriteRender.flipX = (body.velocity.x < 0) || (spriteRender.flipX && body.velocity.x == 0);
        PixelGenerator.facingRight = !spriteRender.flipX;
    }
}
