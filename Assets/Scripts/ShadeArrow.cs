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
    
    
	// Use this for initialization
	void Start () {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        body = gameObject.GetComponent<Rigidbody2D>();
        pixels = new List<GameObject>();
    }
	
	// Update is called once per frame
	void Update () {

        counter++;
        //Charging up orb
        if (counter == 25)
        {
            movingRight = shadeRanger.GetComponent<SpriteRenderer>().flipX;
            Vector2 velocity = movingRight ? new Vector2(speed, 0) : new Vector2(-1*speed,0);
            body.velocity = velocity;
        }
        //Flying through the air
        else if (counter > 25 && counter < 180) {
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
        else if (counter >= 200)
        {
            if (counter == 200)
            {
                spriteRenderer.sprite = null;
                //TODO: Remove collider at this step
            }

            if (counter < (200 + maxSpawnPixels * 3))
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
        //TODO: Add delete on collision

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
}