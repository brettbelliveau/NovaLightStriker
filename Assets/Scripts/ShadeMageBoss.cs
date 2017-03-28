using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class ShadeMageBoss : MonoBehaviour {

    public static int counter = 0;
    private int textCounter = 0;
    private int waitCounter;
    private int startingLifePoints = 70;
    public int lifePoints;
    private int attackFreezeCounter = 0;
    
    public int floatInterval = 8;
    private int attackAfterFrames = 80;
    private int warpFrames = 20;
    private int attackFreezeDuration = 80;
    private int previousWarp = -1;
    private int nextWarp = -1;
    private int warpCounter = 0;

    private int framesPerPixel = 4;
    private int maxSpawnPixels = 20;

    private SpriteRenderer spriteRender;
    private BoxCollider2D collider;
    private GameObject tempText;

    public Sprite floating;
    public Sprite attacking;
    public Sprite[] dying;
    private List<GameObject> pixels;
    public GameObject topL, topR, botL, botR, mid, 
        gameObject, pixel, scoreText, healthBarObject, middleCollider;
    private Slider healthBar;
    public GameObject[] orbs;
    
    private bool attackFreeze;
    private bool warping;
    private int pixelCounter;
    private int dyingAnimationSpeed = 5;
    private bool flag = true;
    public static bool barInit = false;
    private bool finalWarp = false;

    // Use this for initialization
    void Start () {
        collider = gameObject.GetComponent<BoxCollider2D>();
        spriteRender = gameObject.GetComponent<SpriteRenderer>();
        pixels = new List<GameObject>();
        attackFreeze = false;
        warping = false;
        healthBar = healthBarObject.GetComponent<Slider>();

        lifePoints = startingLifePoints;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale != 1)
            return;

        if (barInit)
            healthBar.value = ((float)lifePoints / (float)startingLifePoints);

        if (tempText != null)
        {
            textCounter = (textCounter + 1) % 240;
            tempText.transform.localPosition = new Vector3
                (0, tempText.transform.localPosition.y + 0.005f, 0);

            if (textCounter == 0)
            {
                tempText.GetComponent<TextMesh>().text = "";
                Destroy(tempText);
                SpawnOutPixelsBoss.dying = false;
                Destroy(gameObject);
                Destroy(this);
            }
        }


        /* Spawn Pixels Here */
        pixelCounter = (pixelCounter + 1) % framesPerPixel;

        if (!warping && lifePoints > 0)
        {
            if (pixelCounter == 0)
            {
                pixels.Add(spawnPixelAtRandomLocation(pixel));

                if (pixels.Count > 20)
                {
                    Destroy(pixels[0]);
                    pixels.RemoveAt(0);
                }
            }
        }

        if (flag && lifePoints < (startingLifePoints / 2))
        {
            attackAfterFrames = 30;
            flag = false;
        }


        /* Sprite Section */

        if (lifePoints == 0 && finalWarp && !warping) {
            
            //Case in which we have already played anim
            if (counter == -100) { }
            
            else
            {
                Player.finishTime = (Time.time - Player.timeLost);
                Player.bossDefeated = true;
                gameObject.GetComponent<BoxCollider2D>().enabled = false;

                if (waitCounter < 60)
                {
                    waitCounter++;
                    collider.enabled = false;
                    spriteRender.sprite = dying[0];
                }

                else
                {
                    if (counter == 0)
                    {
                        tempText = spawnTextAtLocation(new Vector3(0, 0, -1f));
                        Player.addScorePoints(10000);
                        SpawnOutPixelsBoss.dying = true;
                    }

                    counter = (counter + 1) % (dying.Length * dyingAnimationSpeed);

                    if (counter > 0)
                        spriteRender.sprite = dying[(counter / dyingAnimationSpeed)];

                    //Done spawning out
                    if (counter == 0)
                    {
                        spriteRender.sprite = null;
                        counter = -100;
                    }
                }
            }
        }

        //Warping to new location
        else if (warping)
        {
            if (counter++ == 0)
            {
                gameObject.SetActive(false);

                while (nextWarp == previousWarp)
                    nextWarp = Random.Range(0, 4);

                previousWarp = nextWarp;

                if (lifePoints <= 0) {
                    gameObject.transform.parent = mid.transform;
                    finalWarp = true;
                    spriteRender.flipX = false;
                    middleCollider.SetActive(true);
                }

                else if (nextWarp == 0)
                {
                    gameObject.transform.parent = topL.transform;
                    spriteRender.flipX = true;
                }
                else if (nextWarp == 1)
                {
                    gameObject.transform.parent = botL.transform;
                    spriteRender.flipX = true;
                }
                else if (nextWarp == 2)
                {
                    gameObject.transform.parent = botR.transform;
                    spriteRender.flipX = false;
                }
                else if (nextWarp == 3)
                {
                    gameObject.transform.parent = topR.transform;
                    spriteRender.flipX = false;
                }

                gameObject.transform.localPosition = new Vector3(0, 0, 0);

            }

            if (counter == warpFrames)
            {
                counter = 0;
                gameObject.SetActive(true);
                warping = false;
                spriteRender.sprite = floating;

                if (warpFrames > 20 && warpFrames < 200) //just took damage, so last warp was long. But was not killing blow
                {
                    if (lifePoints >= startingLifePoints / 2)
                        warpFrames = 20;
                    else
                        warpFrames = 10;
                }
            }
        }

        //Floating sprite
        else if (!attackFreeze)
        {
            if (counter == 0)
            {
                spriteRender.sprite = floating;
            }

            if (counter++ == attackAfterFrames)
            {
                counter = 0;
                attackFreeze = true;
            }
            else
            {
                //Float Up 1/2 of the time
                if ((counter / (floatInterval) % 2) == 0)
                    gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.GetComponent<Rigidbody2D>().velocity.x, 0.05f);

                //Float Down the other 1/2 of the time
                else
                    gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.GetComponent<Rigidbody2D>().velocity.x, -0.05f);
            }
        }

        //Attacking sprite (attackFreeze == true)
        else
        {
            //Float Up 1/2 of the time
            if ((attackFreezeCounter / (floatInterval) % 2) == 0)
                gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.GetComponent<Rigidbody2D>().velocity.x, -0.05f);

            //Float Down the other 1/2 of the time
            else
                gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.GetComponent<Rigidbody2D>().velocity.x, 0.05f);

            attackFreezeCounter = (attackFreezeCounter + 1) % attackFreezeDuration;

            //Throw projectiles after first warp
            if (attackFreezeCounter == 1 && previousWarp > -1)
            {
                //If below 1/2 hp, increase warp speed and only attack after third warp
                if (lifePoints < (startingLifePoints / 2) && warpCounter++ < 2)
                {
                    attackFreezeCounter = 0;
                }

                //Above 1/2 hp, normal execution
                else
                {
                    warpCounter = 0;
                    spriteRender.sprite = attacking;

                    Vector2 velocity;
                    int orbCounter = 0;
                    //Left side (move orbs to right)
                    if (previousWarp < 2)
                    {
                        velocity = new Vector2(0.4f, -0.8f);
                    }
                    //Right side (move orbs to left)
                    else if (previousWarp > 1)
                    {
                        velocity = new Vector2(-0.4f, -0.8f);
                    }
                    //This case will not happen
                    else
                        velocity = new Vector2(0, 0);

                    foreach (GameObject orb in orbs)
                    {
                        //If moving right, skip last orb
                        if (previousWarp < 2 && orbCounter++ >= orbs.Length - 1)
                            continue;
                        //If moving left, skip first orb
                        else if (previousWarp > 1 && orbCounter++ == 0)
                            continue;

                        var tempOrb = Instantiate(orb, orb.GetComponent<Transform>().position, Quaternion.identity) as GameObject;

                        tempOrb.transform.parent = orb.transform;
                        tempOrb.transform.localPosition = new Vector3(0, 0, 0);
                        tempOrb.transform.parent = null;
                        tempOrb.SetActive(true);
                        tempOrb.GetComponent<Rigidbody2D>().velocity = velocity;
                    }
                }
            }

            //Done attacking, warp
            if (attackFreezeCounter == 0)
            {
                warping = true;
                attackFreeze = false;
                counter = 0;
            }
        }
    }
      
    private GameObject spawnPixelAtRandomLocation(GameObject pixel)
    {
        var location = Vector3.zero;

        location.x = Random.Range(-0.7f, 0.7f);
        location.y = Random.Range(-1.5f, 1f);
        location.z = 2;

        return (spawnPixelAtLocation(pixel, location));
    }

    private GameObject spawnPixelAtLocation(GameObject pixel, Vector3 location)
    {
        var tempPixel = Instantiate(pixel, location, Quaternion.identity) as GameObject;

        tempPixel.transform.parent = gameObject.transform;
        tempPixel.transform.localPosition = location;

        tempPixel.transform.parent = null;
        tempPixel.GetComponent<Rigidbody2D>().gravityScale = -0.03f;
        tempPixel.SetActive(true);

        return tempPixel;
    }

    public void sendDamage(int damage)
    {
        var tempLifePoints = lifePoints - damage;
        lifePoints = tempLifePoints < 0 ? 0 : tempLifePoints;
        counter = 0;
        warpFrames = 100;

        BossHealthBar.changed = true;
        attackFreeze = false;
        attackFreezeCounter = 0;
        warping = true;
        spawnBunchOfPixels(10);
    }

    private void spawnBunchOfPixels(int num)
    {
        for (int i = 0; i < num; i++)
            pixels.Add(spawnPixelAtRandomLocation(pixel));
    }

    private GameObject spawnTextAtLocation(Vector3 location)
    {
        var popUpText = Instantiate(scoreText, location, Quaternion.identity) as GameObject;

        popUpText.transform.parent = gameObject.transform;
        popUpText.transform.localPosition = new Vector3(location.x, location.y + 1.5f, -3f);
        
        popUpText.GetComponent<TextMesh>().text = "10000";

        popUpText.SetActive(true);

        return popUpText;
    }
}
