using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadeKnightOrb : MonoBehaviour {

    public GameObject shadeKnight, pixel;
    public Sprite[] sprites;
    public float speed;

    private int counter = 0;
    private int maxSpawnPixels = 6;
    private int framesPerPixel = 6;
    private List<GameObject> pixels;
    
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D body;
    private bool movingRight;
    public bool delete;

    private Vector2 velocityOnStart; 
    
    
	// Use this for initialization
	void Start () {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        body = gameObject.GetComponent<Rigidbody2D>();
        pixels = new List<GameObject>();
    }
	
	// Update is called once per frame
	void Update () {

        if (Time.timeScale != 1)
            return;

        if ((!delete || shadeKnight != null) && counter < 36)
            movingRight = shadeKnight.GetComponent<SpriteRenderer>().flipX;

        counter++;

        if (counter == 1)
        {
            gameObject.GetComponent<AudioSource>().Play();
        }

        //Charging up orb
        if (counter < 36)
        {   
            if (counter == 1) {
                velocityOnStart = body.velocity;
                body.velocity = new Vector2(0, 0);
            }

            if (counter <= 30)
            {
                spriteRenderer.sprite = sprites[counter / 6];
            }
        }
        //Firing Orb
        else if (counter == 36)
        {   //By setting speed to negative value, we can skip this assignment (for custom orb velocities)
            if (speed > 0)
            {
                Vector2 velocity = movingRight ? new Vector2(speed, 0) : new Vector2(-1 * speed, 0);
                body.velocity = velocity;
            }
            else
                body.velocity = velocityOnStart;
        }
        //Flying through the air
        else if (counter > 36 && counter < 180 && !delete) {
            if (counter % framesPerPixel == 0)
                {
                pixels.Add(spawnPixelAtRandomLocation(pixel));

                if (pixels.Count == maxSpawnPixels)
                {
                    Destroy(pixels[0]);
                    pixels.RemoveAt(0);
                }
            }
        }
        //Else if orb flying too long, delete
        else if (counter >= 180 || delete)
        {
            if (counter == 180)
            {
                spriteRenderer.sprite = null;
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
            }

            if (counter < (180 + maxSpawnPixels * 3))
            {
                if (counter % 3 == 0 && pixels.Count > 0)
                {
                    Destroy(pixels[0]);
                    pixels.RemoveAt(0);
                }
            }
            else
            {
                Destroy(gameObject);
                Destroy(this);
            }
        }
    }

    private GameObject spawnPixelAtRandomLocation(GameObject pixel)
    {
        var location = Vector3.zero;

        location.x = 0;
        location.y = (counter % (framesPerPixel*2)) == 0 ? 
            Random.Range(0.05f, 0.25f) : Random.Range(-0.25f, -0.05f);
        location.z = 2;

        return (spawnPixelAtLocation(pixel, location));
    }

    private GameObject spawnPixelAtLocation(GameObject pixel, Vector3 location)
    {
        var tempPixel = Instantiate(pixel, location, Quaternion.identity) as GameObject;

        tempPixel.transform.parent = gameObject.transform;
        tempPixel.transform.localPosition = location;

        tempPixel.transform.parent = null;
        float x = (body.velocity.x / 3) * 2;
        float y = (body.velocity.y / 3) * 2;
        tempPixel.SetActive(true);
        tempPixel.GetComponent<Rigidbody2D>().velocity = new Vector2(x, y);

        return tempPixel;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Begins deletion process
        delete = true;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        spriteRenderer.sprite = null;
        counter = 180;
    }
}