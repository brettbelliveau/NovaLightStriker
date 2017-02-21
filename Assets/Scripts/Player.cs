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
    public static List<bool> hitFromLeft;
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
    public GameObject damageColliderRight, damageColliderLeft;
    public GameObject swordCollider;

    public static bool spawningBool = false; //Flip these before release
    public static bool spawned = true;       //Flip these before release

    public static bool attackFreeze;
    public static bool takingDamage;
    private int damageFrames = 30;
    private int blinkSpeed = 5;
    public static bool hyperModeActive = true;
    private bool disableFrontCollider;
    public static bool invincibleFrames;
    private int iCounter = 0;
    private bool spriteEnabled = true; 
    

    // Use this for initialization
    void Start() {
        body = gameObject.GetComponent<Rigidbody2D>();
        collider = gameObject.GetComponent<BoxCollider2D>();
        spriteRender = gameObject.GetComponent<SpriteRenderer>();
        transform = gameObject.GetComponent<Transform>();

        attackFreeze = false;
        spriteRender.sprite = null;
        SpawnInOutPixels.spawning = spawningBool;
        SpawnInOutPixels.spawned = spawned;

        hitFromLeft = new List<bool>();

        //Ignore Layer collision for sword (15) and all non-enemy layers
        Physics2D.IgnoreLayerCollision(15, 0, true);
        Physics2D.IgnoreLayerCollision(15, 5, true);
        Physics2D.IgnoreLayerCollision(15, 9, true);
        Physics2D.IgnoreLayerCollision(15, 10, true);
        Physics2D.IgnoreLayerCollision(15, 11, true);
        Physics2D.IgnoreLayerCollision(15, 13, true);

    }

    // Update is called once per frame
    void Update() {

        if (invincibleFrames)
        {
            iCounter = (iCounter + 1) % 40;

            if (iCounter % blinkSpeed == 0)
                spriteEnabled = !spriteEnabled;

            spriteRender.enabled = spriteEnabled;

            if (iCounter == 0)
            {
                invincibleFrames = false;
                spriteRender.enabled = true;
                spriteEnabled = true;
            }
        }

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
        }

        /* Sprite Section */

        //Talking damage sprite
        if (takingDamage)
        {
            counter = (counter + 1) % damageFrames;

            //First frame, set sprite, push back and rotate
            if (counter == 1)
            {
                swordCollider.gameObject.SetActive(false);
                attackFreeze = SwordPixelGenerator.attacking = false;

                //If hit from left, push right. If not, push left
                var x = hitFromLeft[0] ? 0.4f : -0.4f;
                //If moving up, push down.
                var y = body.velocity.y > 0.02 ? -0.02f : 0f;
                body.velocity = new Vector2(x, y);

                spriteRender.flipX = hitFromLeft[0];
                hitFromLeft.Clear();
                //if facing left, tilt left, else right
                var z = spriteRender.flipX ? -10 : 10;
                transform.rotation = Quaternion.Euler(0, 0, z);
                camera.transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            //Every x frames blink character
            if (counter % blinkSpeed == 0)
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
                invincibleFrames = true;
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

            if (counter / attackAnimationSpeed == 1) 
                disableFrontCollider = true;

            if (counter / attackAnimationSpeed >= 8 && counter / attackAnimationSpeed < 12)
                swordCollider.gameObject.SetActive(true);

            else if (counter / attackAnimationSpeed == 12) {
                swordCollider.gameObject.SetActive(false);
            }

            //Around when the sword slash anim has finished
            if (hyperModeActive)
            {
                if (counter / attackAnimationSpeed == attacking.Length - 8
                    && counter % attackAnimationSpeed == 0)
                    generateShockWave();
            }

            //Done attacking
            if (counter == 0)
            {
                attackFreeze = SwordPixelGenerator.attacking = false;
                disableFrontCollider = false;
                damageColliderLeft.SetActive(true);
                damageColliderRight.SetActive(true);
            }
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
                    spriteRender.sprite = spawning[spawning.Length - (counter / spawnAnimationSpeed)-1];
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

    if (disableFrontCollider)
        {
            if (spriteRender.flipX)
            {
                damageColliderLeft.SetActive(false);
                damageColliderRight.SetActive(true);
            }
            else
            {
                damageColliderLeft.SetActive(true);
                damageColliderRight.SetActive(false);
            }
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
        if (!takingDamage && !spawningBool && spawned)
        {
            spriteRender.flipX = (body.velocity.x < 0) || (spriteRender.flipX && body.velocity.x == 0);

            swordCollider.gameObject.GetComponent<BoxCollider2D>().offset = 
                (spriteRender.flipX) ? new Vector2(-0.8f, -0.05f) : new Vector2(0.8f, -0.05f);

            SwordPixelGenerator.facingRight = !spriteRender.flipX;
            ShockWave.facingRight = spriteRender.flipX;
        }
    }

    void generateShockWave()
    {
        float x = SwordPixelGenerator.facingRight ? 0.8f : -0.8f;
        Vector3 position = new Vector3(x, 0.1f, 0);
        var tempWave = Instantiate(shockwave, position, Quaternion.identity) as GameObject;

        tempWave.GetComponent<SpriteRenderer>().flipX = !SwordPixelGenerator.facingRight;
        tempWave.transform.parent = gameObject.transform;
        tempWave.transform.localPosition = position;

        tempWave.transform.parent = null;
        tempWave.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        tempWave.SetActive(true);
    }
}
