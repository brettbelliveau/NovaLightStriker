using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShadeCrawler : MonoBehaviour {

    public int counter = 0;
    private int walkingCounter = 0;
    
    public float speed;
    private int movingAnimationSpeed = 8;
    public int turnAfterFrames = 100;

    private Rigidbody2D body;
    private SpriteRenderer spriteRender;
    
    public Sprite[] moving;
    public GameObject pixel, player;
    public GameObject enemyDamageCollider;
    private List<GameObject> pixels;

    public bool takingDamage;
    public bool movingRight;
    private float x, y, xV, yV;
    private bool startDeleting;
    private int index;
    

    // Use this for initialization
    void Start () {
        body = gameObject.GetComponent<Rigidbody2D>();
        spriteRender = gameObject.GetComponent<SpriteRenderer>();
        pixels = new List<GameObject>();
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
                
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
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

        //Walking sprite
        else
        {
            spriteRender.sprite = moving[(walkingCounter / movingAnimationSpeed) % moving.Length];
            
            walkingCounter = (walkingCounter + 1) % (turnAfterFrames * 2);
            
            if (walkingCounter == turnAfterFrames)
            {
                movingRight = true;
            }
            else if (walkingCounter == 0)
            {
                movingRight = false;
            }
        }   
    }

    void FixedUpdate () {

        //Generate left/right movement
        if (!takingDamage)
        {
            float movementSpeed = movingRight ? speed : speed * -1; 
            body.velocity = new Vector2(movementSpeed, body.velocity.y);
        }
        else
        {
            body.velocity = new Vector2(0, body.velocity.y);
        }
        
        //Left orientation for sprite if going left, or if standing still and already left
        if (!takingDamage)
        {
            spriteRender.flipX = (body.velocity.x > 0) || (spriteRender.flipX && body.velocity.x == 0);
        }
    }

    private GameObject spawnPixelAtFixedLocation(GameObject pixel, int x, int y)
    {
        var location = Vector3.zero;

        location.x = 0.2f * x - 0.425f;
        location.y = 0.07f * y - 0.1f;
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
}
