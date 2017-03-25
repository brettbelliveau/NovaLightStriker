using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowGenerator : MonoBehaviour {

    private float width = 35f;
    public GameObject player, pixel;

    private List<GameObject> pixels;

    private int counter = 0;
    //public int maxSnowPixels;
    public int framesPerPixel;

    // Use this for initialization
    void Start () {
        pixels = new List<GameObject>();
	}

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale != 1)
            return;

        counter = (counter + 1) % (framesPerPixel);

        if (counter % framesPerPixel == 0)
        {
            pixels.Add(spawnPixelAtRandomLocation(pixel));

    /*        if (pixels.Count == maxSnowPixels)
            {
                Destroy(pixels[0]);
                pixels.RemoveAt(0);
            } */
        }
    
    }

    private GameObject spawnPixelAtRandomLocation(GameObject pixel)
    {
        var location = Vector3.zero;

        var maxX = width / 1f;
        var minX = -1 * maxX;
        
        location.x = Random.Range(minX, maxX);
        location.y = 12f;
        location.z = 0;
        
        return (spawnPixelAtLocation(pixel, location));
    }

    private GameObject spawnPixelAtLocation(GameObject pixel, Vector3 location)
    {
        var tempPixel = Instantiate(pixel, location, Quaternion.identity) as GameObject;

        tempPixel.transform.parent = player.transform;
        tempPixel.transform.localPosition = location;
        
        tempPixel.transform.parent = null;
        tempPixel.SetActive(true);
        return tempPixel;
    }
}
