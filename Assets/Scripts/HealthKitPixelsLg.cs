using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthKitPixelsLg : MonoBehaviour {

    //private float width = 0.2f;
    public GameObject kit, pixel;
    
    private List<GameObject> pixels;

    private int counter = 0;
    private bool startDeleting;
    private int coordinateCounter;
    private int maxPixels = 10;
    private int framesPerPixel = 8;

    private float[] xCoordinates;
    private float[] yCoordinates;

    // Use this for initialization
    void Start () {
        pixels = new List<GameObject>();
        //Range for X: -0.5f - 0.25f
        xCoordinates = new float[] { -0.5f, 0.15f, -0.3f, 0.25f, -0.5f, 0.2f, -0.1f};
        //Range for Y: 1f - 1.4f
        yCoordinates = new float[] { 1f, 1.2f, 1.2f, 1f, 1.2f, 1.2f, 1.2f};
	}

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale != 1)
            return;

        counter = (counter + 1) % (framesPerPixel);

        if (counter % framesPerPixel == 0)
        {
            if (!startDeleting)
                pixels.Add(spawnPixelAtRandomLocation(pixel));

            if (pixels.Count == maxPixels || (startDeleting && pixels.Count > 0))
            {
                Destroy(pixels[0]);
                pixels.RemoveAt(0);
            } 

            if (startDeleting && pixels.Count == 0)
            {
                Destroy(gameObject);
                Destroy(this);
            }
        }
    
    }

    private GameObject spawnPixelAtRandomLocation(GameObject pixel)
    {
        var location = Vector3.zero;
        coordinateCounter = (coordinateCounter + 1) % 7;
        location.x = xCoordinates[coordinateCounter];
        location.y = yCoordinates[coordinateCounter];
        location.z = 0;
        
        return (spawnPixelAtLocation(pixel, location));
    }

    private GameObject spawnPixelAtLocation(GameObject pixel, Vector3 location)
    {
        var tempPixel = Instantiate(pixel, location, Quaternion.identity) as GameObject;

        tempPixel.transform.parent = kit.transform;
        tempPixel.transform.localPosition = new Vector3(location.x, location.y, 1f);
        
        tempPixel.transform.parent = null;
        tempPixel.SetActive(true);

        tempPixel.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0.05f);
        
        return tempPixel;
    }
    
    void OnTriggerEnter2D (Collider2D other)
    {
        if (other.gameObject.layer == 9 && Player.lifePoints < 100) //character layer, and lifepoints not maxed
        { 
            Player.addLifePoints(50);
            startDeleting = true;
            gameObject.GetComponent<SpriteRenderer>().sprite = null;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}
