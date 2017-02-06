using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthKitPixelsSm : MonoBehaviour {

    //private float width = 0.2f;
    public GameObject kit, pixel;
    
    private List<GameObject> pixels;

    private int counter = 0;
    private int coordinateCounter;
    private int maxPixels = 4;
    private int framesPerPixel = 20;

    private float[] xCoordinates;
    private float[] yCoordinates;

    private bool startDeleting = false;

    // Use this for initialization
    void Start () {
        pixels = new List<GameObject>();
        xCoordinates = new float[] { -0.5f, -0.1f, -0.35f, 0.1f, -0.42f, 0f};
        yCoordinates = new float[] { 0.5f, 0.7f, 0.7f, 0.5f, 0.7f, 0.7f };
	}

    // Update is called once per frame
    void Update()
    {
        counter = (counter + 1) % (framesPerPixel);

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
}
