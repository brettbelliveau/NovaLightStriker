using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnInOutPixels : MonoBehaviour {

    private float width = 0.35f;
    public GameObject player, pixel;

    private List<GameObject> pixels;

    private int counter = 0;
    public int maxSpawnPixels;
    public int framesPerPixel;

    public static bool spawned;
    public static bool spawning;

    public static float deleteSpot; //= -0.12f;        //Spot where pixels start disappearing for spawn in
    private static float spawnSpot; //= 0.85f;          //Spot where pixels start appearing for spawn out


    // Use this for initialization
    void Start () {
        pixels = new List<GameObject>();
        spawned = false;
        deleteSpot = player.transform.position.y - 0.09f;
        spawnSpot = 1000f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale != 1)
            return;

        if (spawning)
        {
            counter = (counter + 1) % (framesPerPixel);

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
    }

    private GameObject spawnPixelAtRandomLocation(GameObject pixel)
    {
        var location = Vector3.zero;

        var maxX = width / 1.2f;
        var minX = -1 * maxX;

        location.x = Random.Range(minX, maxX);

        if (!spawned) //spawning in
            location.y = Random.Range(3.5f, 4f);
        else //spawning out
        {
            if (spawnSpot == 1000f) //first pixel
                spawnSpot = player.transform.position.y + 3.75f;

            location.y = spawnSpot;
            spawnSpot -= 0.05f;
        }

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

        if (!spawned) //spawning in
        { 
            tempPixel.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -0.8f);
            tempPixel.GetComponent<Rigidbody2D>().gravityScale = 0;
        }
        else //spawning out 
            tempPixel.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 1f);
        return tempPixel;
    }
}
