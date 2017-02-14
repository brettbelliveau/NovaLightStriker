using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointPixelGen : MonoBehaviour {
    
    public GameObject point, pixel;
    public Sprite pointOn, pixelOn;

    //TODO: Make this a global variable
    public bool accessed = false;

    private List<GameObject> pixels;

    private int counter = 0;
    private int coordinateCounter;
    private int maxPixels = 15;
    private int framesPerPixel = 6;

    private float[] xCoordinates;
    private float[] yCoordinates;

    private bool startDeleting = false;

    // Use this for initialization
    void Start () {
        pixels = new List<GameObject>();
        xCoordinates = new float[] { -1f, 1f, -0.3f, 0.5f, -0.8f, 0f, -0.5f, 0.75f};
	}

    // Update is called once per frame
    void Update()
    {
        counter = (counter + 1) % (framesPerPixel);

        Debug.Log("Accessed ? " + accessed);
        //TODO: Make this collider-based when the character steps over
        if (!accessed && pixels.Count >= maxPixels-1)
        {
            //TODO: Again, make this variable global so it can be loaded on death
            accessed = true;
            point.GetComponent<SpriteRenderer>().sprite = pointOn;
            pixel.GetComponent<SpriteRenderer>().sprite = pixelOn;
            foreach (GameObject temp in pixels)
            {
                temp.GetComponent<SpriteRenderer>().sprite = pixelOn;
            }
        }
   
        if (counter % framesPerPixel == 0)
        {
            pixels.Add(spawnPixelAtRandomLocation(pixel));

            if (pixels.Count == maxPixels)
            {
                Destroy(pixels[0]);
                pixels.RemoveAt(0);
            } 
        }
    }

    private GameObject spawnPixelAtRandomLocation(GameObject pixel)
    {
        var location = Vector3.zero;
        coordinateCounter = (coordinateCounter + 1) % 6;
        location.x = xCoordinates[coordinateCounter];
        location.y = -0.4f;
        location.z = 0;
        
        return (spawnPixelAtLocation(pixel, location));
    }

    private GameObject spawnPixelAtLocation(GameObject pixel, Vector3 location)
    {
        var tempPixel = Instantiate(pixel, location, Quaternion.identity) as GameObject;
        
        tempPixel.transform.parent = point.transform;
        tempPixel.transform.localPosition = new Vector3(location.x, location.y, 1f);

        tempPixel.transform.parent = null;
        tempPixel.SetActive(true);

        tempPixel.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0.05f);
        
        return tempPixel;
    }
}
