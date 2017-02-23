using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float speed;
    public static int lifePoints;
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
    private BoxCollider2D swordColliderObject;
    private Transform transform;
    public GameObject camera, shockwave, pixel;

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
    private int damageFrames = 20;
    private int blinkSpeed = 5;
    public static bool hyperModeActive = false;
    private bool disableFrontCollider;
    private bool disableBackCollider;
    public static bool invincibleFrames;
    private int iCounter = 0;
    private bool spriteEnabled = true;
    private bool startDeleting = false;
    private List<GameObject> pixels;
    private float xV, yV;
    private bool earlySwing = false;


    // Use this for initialization
    void Start() {
        body = gameObject.GetComponent<Rigidbody2D>();
        collider = gameObject.GetComponent<BoxCollider2D>();
        spriteRender = gameObject.GetComponent<SpriteRenderer>();
        transform = gameObject.GetComponent<Transform>();
        pixels = new List<GameObject>();

        attackFreeze = false;
        spriteRender.sprite = null;
        SpawnInOutPixels.spawning = spawningBool;
        SpawnInOutPixels.spawned = spawned;

        swordColliderObject = swordCollider.gameObject.GetComponent<BoxCollider2D>();

        hitFromLeft = new List<bool>();

        lifePoints = 100;

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
            iCounter = (iCounter + 1) % 50;

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
            if (lifePoints <= 0)
                playDyingAnimation();
            else
            {
                counter = (counter + 1) % damageFrames;

                //First frame, set sprite, push back and rotate
                if (counter == 1)
                {
                    earlySwing = true;
                    disableFrontCollider = disableBackCollider = false;
                    //For now, take 10 damage on every hit
                    lifePoints -= 10;
                    Debug.Log("Life Points = " + lifePoints);
                    if (lifePoints <= 0)
                        playDyingAnimation();

                    else
                    {
                        swordCollider.gameObject.SetActive(false);
                        attackFreeze = SwordPixelGenerator.attacking = false;

                        //If hit from left, push right. If not, push left
                        //var x = hitFromLeft[0] ? 0.2f : -0.2f;
                        //If moving up, push down.
                        var y = body.velocity.y > 0.02 ? -0.02f : 0f;
                        body.velocity = new Vector2(0, y);
                        
                        hitFromLeft.Clear();
                        //if facing left, tilt left, else right
                    }
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
                }
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

            //After 4 frames, invincible from front and back
            if (counter == 4)
            {
                disableBackCollider = true;
                disableFrontCollider = true;
            }


            //After 5th sprite frame, not invincible from back
            if (counter / attackAnimationSpeed == 5)
                disableBackCollider = false;

            //After 4th sprite frame, turn on sword collider 
            if (counter / attackAnimationSpeed == 4)
            {
                swordCollider.gameObject.SetActive(true);
                earlySwing = true;
            }

            //After 9th sprite frame, move sword collider away from character
            else if (counter / attackAnimationSpeed == 9)
            {
                earlySwing = false;
            }

            //After 12th sprite frame, turn off sword collider 
            else if (counter / attackAnimationSpeed == 12)
            {
                swordCollider.gameObject.SetActive(false);
                earlySwing = true;
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
                else
                {
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
                    spriteRender.sprite = spawning[spawning.Length - (counter / spawnAnimationSpeed) - 1];
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
            if (!spriteRender.flipX)
            {
                damageColliderRight.SetActive(false);
                damageColliderLeft.SetActive(!disableBackCollider);
            }
            else
            {
                damageColliderLeft.SetActive(false);
                damageColliderRight.SetActive(!disableBackCollider);
            }
        }

        if (earlySwing)
        {
            swordColliderObject.offset = (spriteRender.flipX) ? new Vector2(0f, -0.05f) : new Vector2(0f, -0.05f);
            swordColliderObject.size = new Vector2(0.3f, 0.7f);
        }
        else
        {
            swordColliderObject.offset = (spriteRender.flipX) ? new Vector2(-0.65f, -0.05f) : new Vector2(0.65f, -0.05f);
            swordColliderObject.size = new Vector2(0.3f, 1.8f);
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

    void playDyingAnimation()
    {
        counter = (counter + 1) % 180;
        if (counter == 2 && !startDeleting)
        {
            body.velocity = new Vector3(0, 0, 0);
            body.gravityScale = 0;
            collider.enabled = false;
            damageColliderLeft.SetActive(false);
            damageColliderRight.SetActive(false);
            swordCollider.SetActive(false);
            spriteRender.sprite = null;

            xV = 0;
            yV = 0.1f;

            int r = 0;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                    pixels.Add(spawnPixelAtFixedLocation(pixel, i, j));
                }
            }
        }
        else if (counter == 0 || startDeleting)
        {
            startDeleting = true;
            if (pixels.Count > 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    int index = pixels.Count - 1;
                    if (index >= 0)
                    {
                        Destroy(pixels[index]);
                        pixels.RemoveAt(index);
                    }
                }
            }
            else
            {
                //Reload from checkpoint, or game over
            }
        }
        else
            body.velocity = new Vector3(0, 0, 0);
    }

    private GameObject spawnPixelAtFixedLocation(GameObject pixel, int x, int y)
    {
        var location = Vector3.zero;

        location.x = 0.075f * x - 0.425f;
        location.y = 0.07f * y - 1.1f;
        location.z = 0f;

        return (spawnPixelAtLocation(pixel, location));
    }

    private GameObject spawnPixelAtLocation(GameObject pixel, Vector3 location)
    {
        var tempPixel = Instantiate(pixel, location, Quaternion.identity) as GameObject;

        tempPixel.transform.parent = gameObject.transform;
        tempPixel.transform.localPosition = location;

        tempPixel.transform.parent = null;
        tempPixel.SetActive(true);

        var xVelocity = Random.Range(xV - 0.08f, xV + 0.08f);
        var yVelocity = Random.Range(yV, yV + 0.05f);

        tempPixel.GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity, yVelocity);
        tempPixel.GetComponent<Rigidbody2D>().gravityScale = Random.Range(-0.035f, 0.02f);

        return tempPixel;
    }

    public static void addLifePoints(int add)
    {
        lifePoints += add;
        lifePoints = lifePoints > 100 ? 100 : lifePoints;
        Debug.Log("Life Points = " + lifePoints);
    }
}
