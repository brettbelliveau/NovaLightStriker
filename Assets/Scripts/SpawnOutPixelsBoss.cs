using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOutPixelsBoss : MonoBehaviour {

    private float width = 0.35f;
    public GameObject boss, pixel;

    private List<GameObject> pixels;

    private int counter = 0;
    private int maxSpawnPixels = 250;
    private int framesPerPixel = 1;
    
    public static bool dying;
    private bool started;
    
    private static float spawnSpot; // 0.85f;          //Spot where pixels start appearing for spawn out


    // Use this for initialization
    void Start () {
        pixels = new List<GameObject>();
        spawnSpot = 1000f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale != 1)
            return;

        counter = (counter + 1) % (framesPerPixel);

        if (counter % framesPerPixel == 0)
        {
            if (dying && ShadeMageBoss.counter > -100)
            {
                pixels.Add(spawnPixelAtRandomLocation(pixel));
                pixels.Add(spawnPixelAtRandomLocation(pixel));
            }
            if (pixels.Count == maxSpawnPixels || (started && pixels.Count > 0))
            {
                started = true;
                Destroy(pixels[0]);
                pixels.RemoveAt(0);
                
                if (pixels.Count < 100)
                {
                    GameObject.FindObjectOfType<ScoreRecap>().run = true;
                }
            }
        }
        
    }

    private GameObject spawnPixelAtRandomLocation(GameObject pixel)
    {
        var location = Vector3.zero;

        var maxX = width / 1f;
        var minX = -1 * maxX;

        location.x = Random.Range(minX, maxX-0.2f);

        if (spawnSpot == 1000f) //first pixel
            spawnSpot = boss.transform.position.y + 4.5f;

        location.y = spawnSpot;
        spawnSpot -= 0.0075f;
        
        location.z = 2;

        return (spawnPixelAtLocation(pixel, location));
    }

    private GameObject spawnPixelAtLocation(GameObject pixel, Vector3 location)
    {
        var tempPixel = Instantiate(pixel, location, Quaternion.identity) as GameObject;

        tempPixel.transform.parent = boss.transform;
        tempPixel.transform.localPosition = location;
        
        tempPixel.transform.parent = null;
        tempPixel.SetActive(true);

        if (!boss) //spawning in
        { 
            tempPixel.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -0.8f);
            tempPixel.GetComponent<Rigidbody2D>().gravityScale = 0;
        }
        else //spawning out 
            tempPixel.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-0.2f, 0.2f), Random.Range(-0.5f, 0.5f));
        return tempPixel;
    }
}
