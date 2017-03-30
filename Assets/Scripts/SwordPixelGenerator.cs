using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPixelGenerator : MonoBehaviour {

    private float width = 1f;
    public GameObject player, pixel;
    
    public static bool facingRight;
    public static bool attacking;

    private List<GameObject> pixels;

    private int counter = 0;
    public int maxPixels;
    public int framesPerPixel;
    
    private bool startDeleting = false;

    // Use this for initialization
    void Start () {
        pixels = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {

        if (Time.timeScale != 1)
            return;

        if (attacking || pixels.Count > 0 && !Player.hyperModeActive)
            managePixels();
	}

    private void managePixels()
    {
        counter = (counter + 1) % (framesPerPixel * maxPixels);
        //At the right frame (if still attacking), spawn pixel
        if (attacking && counter % framesPerPixel == 0)
            pixels.Add(spawnPixelAtRandomLocation(pixel));

        //Start deleting flag set true when pixel count gets to max
        startDeleting |= pixels.Count == maxPixels;

        //If done attacking, or pixel count reaches max
        //Start deleting pixels!
        if (!attacking && counter % framesPerPixel == 0 
            || startDeleting && counter % framesPerPixel == 0) {
            Destroy(pixels[0]);
            pixels.RemoveAt(0);
        }

        //Turn off startDeleting when we are done attacking
        //Fixes a bug where pixels are always delted after first run
        startDeleting &= attacking;
    }

    private GameObject spawnPixelAtRandomLocation(GameObject pixel)
    {
        var location = Vector3.zero;

        var maxX = width / 1f;
        var minX = 0.5f;

        var maxY = 0f;
        var minY = 0f;
        bool active = true;

        //For attacking animation, sword-swinging is during
        //frames 9-13. We want to spawn pixels then for best effect

        var frame = Player.counter / Player.attackAnimationSpeed;

        if (frame >= 9 && frame <= 10)
        {
            maxY = 1f;
            minY = 0f;
        }
        else if (frame >= 11 && frame <= 13)
        {
            maxY = 0f;
            minY = -1f;
        }
        else
            active = false;

        //Spawn pixels on forward side during attack
        maxX = facingRight ? maxX : maxX * -1;
        minX = facingRight ? minX : minX * -1;

        location.x = Random.Range(minX, maxX);
        location.y = Random.Range(minY, maxY);
        location.z = 0;
        
        return (spawnPixelAtLocation(pixel, location, active));
    }

    private GameObject spawnPixelAtLocation(GameObject pixel, Vector3 location, bool active)
    {
        var tempPixel = Instantiate(pixel, location, Quaternion.identity) as GameObject;
        tempPixel.SetActive(active);

        tempPixel.transform.parent = player.transform;
        tempPixel.transform.localPosition = location;
        
        tempPixel.transform.parent = null;
        return tempPixel;
    }
}
