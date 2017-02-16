using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    
    public float speed;
    public bool onGround;
    public static int counter = 0;
    public int runAnimationSpeed = 3;
    public static int attackAnimationSpeed = 2;
    private static int spawnAnimationSpeed = 8;
    private int framesSinceOnGround = 0;
    private bool jumped = false;
    private static int waitFrames = 40; //number of frames to wait before spawning

    private Rigidbody2D body;
    private SpriteRenderer spriteRender;
    private BoxCollider2D collider;
    private Transform transform;
    public GameObject camera, shockwave;

    public Sprite standing, jumping;
    public Sprite[] running;
    public Sprite[] attacking;
    public Sprite[] spawning;
    public Sprite damageSprite;

    public bool spawningBool = true;
    public bool spawned = false;

    private bool takingDamage;
    private bool attackFreeze;
    private int damageFrames = 40;
    private int blinkSpeed = 4;
    public static bool hyperModeActive;

    private int damageCounter = 0;

    // Use this for initialization
    void Start () {
        body = gameObject.GetComponent<Rigidbody2D>();
        collider = gameObject.GetComponent<BoxCollider2D>();
        spriteRender = gameObject.GetComponent<SpriteRenderer>();
        transform = gameObject.GetComponent<Transform>();
        attackFreeze = false;
        spriteRender.sprite = null;
        SpawnInOutPixels.spawning = spawningBool;
        SpawnInOutPixels.spawned = spawned;
    }

    // Update is called once per frame
    void Update() {
        //damageCounter = (damageCounter + 1) % 300;
       
        if (!spawningBool && spawned)
        {
            //Check if back on ground after jump/fall
            if (!onGround && Mathf.Abs(body.velocity.y) < 0.02) {
                onGround = true;
                framesSinceOnGround = 0;
                jumped = false;
            }

            //Turn off onGround when falling from ledge
            if (onGround)
                onGround = Mathf.Abs(body.velocity.y) < 0.02;
            //Use this counter as a check for how long since fallen off ledge
            else
                framesSinceOnGround++;
            
            //Give the user 2 frames after leaving platform to jump
            if (Input.GetButtonDown("Jump") && (onGround || (!jumped && framesSinceOnGround < 2)) && !attackFreeze && !takingDamage)
            {
                body.AddForce(Vector2.up * 14);
                onGround = false;
                jumped = true;
            }

            if (Input.GetButtonDown("Fire1") && !attackFreeze && !takingDamage)
            {
                counter = 0;
                attackFreeze = true;
                SwordPixelGenerator.attacking = true;
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
        else if (!attackFreeze && body.velocity.x == 0 && Mathf.Abs(body.velocity.y) < 0.02 && !spawningBool && spawned)
        {
            spriteRender.sprite = standing;
        }

        //Running sprite
        else if (!attackFreeze && body.velocity.x != 0 && Mathf.Abs(body.velocity.y) < 0.02 && !spawningBool && spawned)
        {
            counter = (counter + 1) % (running.Length * runAnimationSpeed);
            spriteRender.sprite = running[counter / runAnimationSpeed];
        }

        //Attacking sprite
        else if (attackFreeze)
        {
            counter = (counter + 1) % (attacking.Length * attackAnimationSpeed);
            spriteRender.sprite = attacking[counter / attackAnimationSpeed];

            //Around when the sword slash anim has finished
            if (hyperModeActive)
            {
                if (counter / attackAnimationSpeed == attacking.Length - 8
                    && counter % attackAnimationSpeed == 0)
                    generateShockWave();
            }
           
            //Done attacking
            if (counter == 0)
                attackFreeze = SwordPixelGenerator.attacking = false;
        }

        else if (spawningBool)
        {
            if (!spawned) //spawn-in animation
            {
                if (counter < waitFrames)
                {
                    counter++;
                    if (counter >= waitFrames)
                    {
                        waitFrames = -1;
                        counter = 0;
                    }
                }
                else { 
                counter = (counter + 1) % (spawning.Length * spawnAnimationSpeed);
                spriteRender.sprite = spawning[counter / spawnAnimationSpeed];
                    //Done spawning
                    if (counter == 0)
                    {
                        spriteRender.sprite = standing;
                        spawningBool = false;               //Comment this out to test spawn out
                        spawned = true;
                        SpawnInOutPixels.spawning = false;  //Comment this out to test spawn out
                        SpawnInOutPixels.spawned = true;
                    }
                }
            }
            else //spawn-out animation
            {
                counter = (counter + 1) % (spawning.Length * spawnAnimationSpeed);
                if (counter > 0)
                    spriteRender.sprite = spawning[spawning.Length - (counter / spawnAnimationSpeed)];
                //Done spawning out
                if (counter == 0)
                {
                    spriteRender.sprite = null;
                    spawningBool = false;
                    spawned = false;
                    SpawnInOutPixels.spawning = false;
                    SpawnInOutPixels.spawned = false;
                }
            }
        }

        //Jumping sprite
        else if (body.velocity.y != 0)
        {
            spriteRender.sprite = jumping;
        }
    
    }

    void FixedUpdate () {

        //Obtain left/right input
        if (!spawningBool && spawned && !takingDamage)
        {
            float h = Input.GetAxisRaw("Horizontal");
            body.velocity = new Vector2(speed * h, body.velocity.y);
        }

        //Turn off collider when moving upward
        //Layer 9 is character, layer 11 is special platforms
        Physics2D.IgnoreLayerCollision(9, 11, body.velocity.y >= 0.02);

        //Left orientation for sprite if going left, or if standing still and already left
        if (!spawningBool && spawned && !takingDamage)
        {
            spriteRender.flipX = (body.velocity.x < 0) || (spriteRender.flipX && body.velocity.x == 0);
            SwordPixelGenerator.facingRight = !spriteRender.flipX;
            ShockWave.facingRight = spriteRender.flipX;
        }
    }

    void generateShockWave()
    {
        float x = SwordPixelGenerator.facingRight ? 0.8f : -0.8f;
        Vector3 position = new Vector3(x, 0.1f, 10);
        var tempWave = Instantiate(shockwave, position, Quaternion.identity) as GameObject;

        tempWave.GetComponent<SpriteRenderer>().flipX = !SwordPixelGenerator.facingRight;
        tempWave.transform.parent = gameObject.transform;
        tempWave.transform.localPosition = position;

        tempWave.transform.parent = null;
        tempWave.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        tempWave.SetActive(true);
    }
}
