using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointPixelGen : MonoBehaviour {
    
    public GameObject bottom, top, pixel;
    public Sprite pointOn, pixelOn;

    //TODO: Make this a global variable
    public bool spawn = true;

    private List<GameObject> pixels;

    private int counter = 0;
    private float pixelCounter;
    private int coordinateCounter;
    private int maxPixels = 30;
    private int framesPerPixel = 3;
    public bool checkPointOne;

    private float[] xCoordinates;
    private float[] yCoordinates;

    // Use this for initialization
    void Start () {
        pixels = new List<GameObject>();
        xCoordinates = new float[] { -1f, 1f, -0.3f, 0.5f, -0.8f, 0f, -0.5f, 0.75f};
        if (checkPointOne && "True".Equals(PlayerPrefs.GetString("CheckPointOne")))
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            turnEverythingGreen();
        }
        else if ("True".Equals(PlayerPrefs.GetString("CheckPointTwo")))
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            turnEverythingGreen();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (spawn)
        {
            counter = (counter + 1) % (framesPerPixel);

            if (counter == 0)
            {
                pixels.Add(spawnPixelAtRandomLocation(pixel));

                if (pixels.Count == maxPixels)
                {
                    Destroy(pixels[0]);
                    pixels.RemoveAt(0);
                }
            }
        }
    }
    private GameObject spawnPixelAtRandomLocation(GameObject pixel)
    {
        pixelCounter = Random.Range(0, 1f);
        var location = Vector3.zero;
        coordinateCounter = (coordinateCounter + 1) % 6;
        location.x = xCoordinates[coordinateCounter];
        location.y = pixelCounter > 0.5f ? -0.4f : -0.6f;
        location.z = 0;
        
        return (spawnPixelAtLocation(pixel, location));
    }

    private GameObject spawnPixelAtLocation(GameObject pixel, Vector3 location)
    {
        var tempPixel = Instantiate(pixel, location, Quaternion.identity) as GameObject;

        if (pixelCounter > 0.5f)
            tempPixel.transform.parent = bottom.transform;
        else
            tempPixel.transform.parent = top.transform;

        tempPixel.transform.localPosition = new Vector3(location.x, location.y, 1f);

        tempPixel.transform.parent = null;
        tempPixel.SetActive(true);

        Rigidbody2D body = tempPixel.GetComponent<Rigidbody2D>();

        if (pixelCounter > 0.5f)
        {   
            body.velocity = new Vector2(0, 0.05f);
        }
        else
        {
            body.velocity = new Vector2(0, -0.05f);
            body.gravityScale *= -1;
        }

        return tempPixel;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Player layer only
        if (other.gameObject.layer == 9)
        {
           gameObject.GetComponent<BoxCollider2D>().enabled = false;

            if (checkPointOne)
                Player.checkPointOne = true;
            else
                Player.checkPointTwo = true;

            Player.timeAtCheckPoint = Time.time;
            PlayerPrefs.SetInt("Score", Player.score);
            PlayerPrefs.SetInt("TotalScore", Player.totalScore);

            turnEverythingGreen();
        }
    }

    private void turnEverythingGreen()
    {
        top.GetComponent<SpriteRenderer>().sprite = pointOn;
        bottom.GetComponent<SpriteRenderer>().sprite = pointOn;
        pixel.GetComponent<SpriteRenderer>().sprite = pixelOn;

        foreach (GameObject temp in pixels)
        {
            temp.GetComponent<SpriteRenderer>().sprite = pixelOn;
        }
    }
}
