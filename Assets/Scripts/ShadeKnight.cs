using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShadeKnight : MonoBehaviour {

    public int counter = 0;
    private int walkingCounter = 0;
    private int attackFreezeCounter = 0;
    
    public float speed;
    public int floatInterval = 8;
    public int turnAfterFrames = 120;
    private int attackAnimationSpeed = 3;
    private int attackFreezeDuration = 40;

    private Rigidbody2D body;
    private SpriteRenderer spriteRender;
    private BoxCollider2D damageCollider;
    
    public Sprite moving;
    public Sprite[] attacking;
    public GameObject orb, pixel, pixel1, pixel2, player;
    public GameObject enemyDamageCollider;
    private List<GameObject> pixels;

    public bool takingDamage;
    private bool attackFreeze;
    public bool movingRight;
    private float x, y, xV, yV;
    private bool startDeleting;
    private int index;

    // Use this for initialization
    void Start () {
        body = gameObject.GetComponent<Rigidbody2D>();
        spriteRender = gameObject.GetComponent<SpriteRenderer>();
        damageCollider = enemyDamageCollider.GetComponent<BoxCollider2D>();
        pixels = new List<GameObject>();
        attackFreeze = false;
        takingDamage = false;
        movingRight = false;

        //Make enemies ignore collisions with each other/projectiles
        Physics2D.IgnoreLayerCollision(14, 14, true);
        //Make enemies ignore collisions with VFX like snow
        Physics2D.IgnoreLayerCollision(10, 14, true);

        //Make projectiles ignore VFX
        Physics2D.IgnoreLayerCollision(10, 12, true);        
        
        //Fix glitch with floating right over time
        if (turnAfterFrames % 2 == 1)
            turnAfterFrames -= 1;
    }

    // Update is called once per frame
    void Update() {

        if (Time.timeScale != 1)
            return;

        //Talking damage (must reset counter first)
        if (takingDamage)
        {
            counter = (counter + 1) % 40;
            if (counter == 1 && !startDeleting)
            {
                body.velocity = new Vector2(0, 0);
                spriteRender.sprite = null;

                //Increase velocity depending on dist. to player
                x = gameObject.transform.position.x - player.transform.position.x;
                y = gameObject.transform.position.y - player.transform.position.y + 0.137f;

                xV = -0.1f * (x * x);
                yV = 0.3f * (y * y);

                int r = 0;
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 30; j++)
                    {
                        r = Random.Range(0, 2);
                        pixel = r == 0 ? pixel1 : pixel2;
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
                        index = pixels.Count - 1;
                        if (index >= 0)
                        {
                            Destroy(pixels[index]);
                            pixels.RemoveAt(index);
                        }
                    }
                }
                else
                {
                    Destroy(gameObject);
                    Destroy(this);
                }
            }

        }
        //Standing sprite
        else if (!attackFreeze)
        {
            //Float down 1/2 of the time
            if ((walkingCounter / (turnAfterFrames*2 / floatInterval) % 2) == 0)
                gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.GetComponent<Rigidbody2D>().velocity.x, -0.02f);

            //Float up the other 1/2 of the time
            else
                gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.GetComponent<Rigidbody2D>().velocity.x, 0.02f);

            walkingCounter = (walkingCounter + 1) % (turnAfterFrames * 2);
            if (walkingCounter == turnAfterFrames)
            {
                movingRight = true;
                damageCollider.offset = new Vector2(0.1f, 0);
            }
            else if (walkingCounter == 0)
            {
                movingRight = false;
                damageCollider.offset = new Vector2(-0.1f, 0);
            }

            //Attack if player within certain distances
            else if (walkingCounter % 10 == 0)
            {
                if (playerIsNear())
                {
                    attackFreeze = true;

                    float x = movingRight ? 0.6f : -0.6f;
                    Vector3 orbPosition = new Vector3(x, -0.5f, -5);
                    var tempOrb = Instantiate(orb, orbPosition, Quaternion.identity) as GameObject;

                    tempOrb.transform.parent = gameObject.transform;
                    tempOrb.transform.localPosition = orbPosition;

                    tempOrb.transform.parent = null;
                    tempOrb.SetActive(true);
                }
            }
        }
        

        //Attacking sprite
        else
        {
            //Stop movement
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

            if (attackFreezeCounter == 0)
                counter = (counter + 1) % (attacking.Length * attackAnimationSpeed);

            if (counter > 0)
                spriteRender.sprite = attacking[counter / attackAnimationSpeed];

            //Done with first attack frames
            if (counter == 0)
            {
                attackFreezeCounter = (attackFreezeCounter + 1) % attackFreezeDuration;
                if (attackFreezeCounter == 0)
                {
                    attackFreeze = false;
                    spriteRender.sprite = moving;
                }
            }
        }
        
    }

    void FixedUpdate () {

        //Generate left/right movement
        if (!attackFreeze && !takingDamage)
        {
            float movementSpeed = movingRight ? speed : speed * -1; 
            body.velocity = new Vector2(movementSpeed, body.velocity.y);
        }
        else if (attackFreeze)
        {
            body.velocity = new Vector2(0, body.velocity.y);
        }
        
        //Left orientation for sprite if going left, or if standing still and already left
        if (!attackFreeze && !takingDamage)
        {
            spriteRender.flipX = (body.velocity.x > 0) || (spriteRender.flipX && body.velocity.x == 0);
        }
    }
    private GameObject spawnPixelAtFixedLocation(GameObject pixel, int x, int y)
    {
        var location = Vector3.zero;

        location.x = 0.075f * x - 0.425f;
        location.y = 0.07f * y - 1.1f;
        location.z = Random.Range(2f, 3f);

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

    private bool playerIsNear()
    {
        //Calculate distance to player from body
        x = gameObject.transform.position.x - player.transform.position.x;
        y = gameObject.transform.position.y - player.transform.position.y;

        if (System.Math.Abs(y) < 0.1f)
        {
            //Player is on the right and enemy unit is facing right
            if (x > -2.4f && x < 0 && movingRight)
                return true;

            //Player is on the left and enemy unit is facing left
            else if (x < 2.4f && x > 0 && !movingRight)
                return true;

            else
                return false;
        }

        else
            return false;
    }
}
