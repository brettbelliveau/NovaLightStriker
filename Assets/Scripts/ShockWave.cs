using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWave : MonoBehaviour {

    public GameObject player, pixel;
    public float speed;

    private int counter = 0;
    private int maxSpawnPixels = 10;
    private int framesPerPixel = 2;
    private List<GameObject> pixels;
    
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D body;
    public static bool facingRight;
    
    
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

        counter++;
        //Flying through the air
        if (counter > 0 && counter < 60) {
            
            body.velocity = !spriteRenderer.flipX ? new Vector2(speed, 0) : new Vector2(-1 * speed, 0);
            
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
        else if (counter >= 60)
        {
            if (counter == 60)
            {
                spriteRenderer.sprite = null;
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
            }

            if (counter < (60 + maxSpawnPixels * 3))
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

        location.x = facingRight ? 0.2f : -0.2f;
        location.y = Random.Range(-1f, 1.15f);
        location.z = 0;

        return (spawnPixelAtLocation(pixel, location));
    }

    private GameObject spawnPixelAtLocation(GameObject pixel, Vector3 location)
    {
        var tempPixel = Instantiate(pixel, location, Quaternion.identity) as GameObject;

        tempPixel.transform.parent = gameObject.transform;
        tempPixel.transform.localPosition = location;

        tempPixel.transform.parent = null;
        float x = (body.velocity.x/3)*2;
        tempPixel.SetActive(true);
        tempPixel.GetComponent<Rigidbody2D>().velocity = new Vector2(x, 0);
        tempPixel.GetComponent<Rigidbody2D>().gravityScale = 0;

        return tempPixel;
    }
}