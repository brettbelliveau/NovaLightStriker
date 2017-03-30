using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadeArrow : MonoBehaviour {

    public GameObject shadeRanger, pixel;
    public Sprite[] sprites;
    public float speed;

    private int counter = 0;
    private int maxSpawnPixels = 6;
    private int framesPerPixel = 9;
    private List<GameObject> pixels;
    
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D body;
    private bool movingRight;
    public bool delete;
    
	// Use this for initialization
	void Start () {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        body = gameObject.GetComponent<Rigidbody2D>();
        pixels = new List<GameObject>();
        counter = 0;
        delete = false;
    }
	
	// Update is called once per frame
	void Update () {

        if (Time.timeScale != 1)
            return;

        counter++;
        //After charged up arrow
        
        if (counter == 25)
        {
            movingRight = shadeRanger.GetComponent<SpriteRenderer>().flipX;
            Vector2 velocity = movingRight ? new Vector2(speed, 0) : new Vector2(-1*speed,0);
            body.velocity = velocity;
        }
        //Flying through the air
        else if (counter > 25 && counter < 180 && !delete) {
            if (counter % framesPerPixel == 0)
                {
                pixels.Add(spawnPixelAtRandomLocation(pixel));

                if (pixels.Count == maxSpawnPixels)
                {
                    Destroy(pixels[0]);
                    pixels.RemoveAt(0);
                }
            }

            //TODO: Check if hit another object
        }
        //Else if orb flying too long (or collision), ;delete
        else if (counter >= 180 || delete)
        {
            if (counter == 180)
            {
                spriteRenderer.sprite = null;
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                //TODO: Remove collider at this step
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
        
        location.x = movingRight ? -0.6f : 0.6f;
        location.y = (counter % (framesPerPixel * 2)) == 0 ?
            Random.Range(-0.6f, -0.5f) : Random.Range(-0.3f, -0.2f) ;
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
        tempPixel.SetActive(true);
        tempPixel.GetComponent<Rigidbody2D>().velocity = new Vector2(x, 0);

        return tempPixel;
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        //Begins deletion process
        delete = true;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        spriteRenderer.sprite = null;
        counter = 180;
    }
}