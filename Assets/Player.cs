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
    private Transform transform;
    public GameObject camera;

    public Sprite standing, jumping;
    public Sprite[] running;
    public Sprite[] attacking;
    public Sprite damageSprite;

    private bool takingDamage;
    private bool attackFreeze;
    private int damageFrames = 40;
    private int blinkSpeed = 4;

    private int damageCounter = 0;

    // Use this for initialization
    void Start () {
        body = gameObject.GetComponent<Rigidbody2D>();
        collider = gameObject.GetComponent<BoxCollider2D>();
        spriteRender = gameObject.GetComponent<SpriteRenderer>();
        transform = gameObject.GetComponent<Transform>();
        attackFreeze = false;
	}
	
	// Update is called once per frame
	void Update () {

        damageCounter = (damageCounter + 1) % 300;

        //Check if back on ground after jump/fall
        if (!onGround && Mathf.Abs(body.velocity.y) < 0.02)
            onGround = true;
        
        //Turn off onGround when falling from ledge
        if (onGround)
            onGround = Mathf.Abs(body.velocity.y) < 0.02;

        if (Input.GetButtonDown("Jump") && onGround && !attackFreeze && !takingDamage)
        {
            body.AddForce(Vector2.up * 170);
            onGround = false;
        }

        if (Input.GetButtonDown("Fire1") && !attackFreeze && !takingDamage)
        {
            counter = 0;
            attackFreeze = true;
            PixelGenerator.attacking = true;
        }

        //TODO: Scrap damageCounter, check for enemy collision
        if (damageCounter == 100 && !takingDamage)
        {
            counter = 0;
            takingDamage = true;

            //If facing left, push right. If not, push left
            var x = spriteRender.flipX ? 3f : -3f;
            var y = body.velocity.y > 0.02 ? -0.02f : 0f; 
            body.velocity = new Vector2(x, y);
        }
        
        /* Sprite Section */

        //Talking damage sprite
        if (takingDamage)
        {
            counter = (counter + 1) % damageFrames;
            
            //First frame, set sprite and rotate
            if (counter == 1)
            {
                //if facing left, tilt left, else right
                var z = spriteRender.flipX ? -10 : 10; 
                transform.rotation = Quaternion.Euler(0, 0, z);
                camera.transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            //Every x frames blink character
            if (counter % blinkSpeed * 2 > blinkSpeed)
            {
                spriteRender.sprite = damageSprite;
            }
            else
            {
                spriteRender.sprite = null;
            }

            //Done taking damage
            if (counter == 0)
            {
                takingDamage = false;
                transform.rotation = Quaternion.Euler(0, 0, 0);
                camera.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }

        //Standing sprite
        else if (!attackFreeze && body.velocity.x == 0 && Mathf.Abs(body.velocity.y) < 0.02)
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
        if (!takingDamage)
        {
            float h = Input.GetAxisRaw("Horizontal");
            body.velocity = new Vector2(speed * h, body.velocity.y);
        }
        
        //Turn off collider when moving upward
        //TODO: Change this to layering system
        collider.enabled = body.velocity.y <= 0.02;

        //Left orientation for sprite if going left, or if standing still and already left
        if (!takingDamage)
        {
            spriteRender.flipX = (body.velocity.x < 0) || (spriteRender.flipX && body.velocity.x == 0);
            PixelGenerator.facingRight = !spriteRender.flipX;
        }
    }
}
