using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class SkeletonBoss : MonoBehaviour {

    public static int counter = 0;
    private int textCounter = 0;
    private int waitCounter;
    private int startingLifePoints = 70;
    public int lifePoints;
    private int attackFreezeCounter = 0;
    
    public int standInterval = 8;
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

    public Sprite standing;
    public Sprite[] attacking;
    public Sprite[] dying;
    private List<GameObject> pixels;
    public GameObject Left, Right, Middle, MiddleCollider, gameObject, pixel, scoreText, healthBarObject, 
                      skeletonShockwave, skeletonSwordCollider;
    private Slider healthBar;
    //public GameObject shockwave;
    
    private bool attackFreeze;
    private bool warping;
    private int pixelCounter;
    private int dyingAnimationSpeed = 5;
    private int attackingAnimationSpeed = 5;
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

        //swordColliderObject = swordCollider.gameObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale != 1)
        {
            print("Time scale error");
            return;
        }

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

        if (lifePoints == 0 && finalWarp && !warping)
        {

            //Case in which we have already played anim
            if (counter == -100) { }

            else
            {
                Player.finishTime = (Time.time - Player.timeLost);
                Player.bossDefeated = true;
                gameObject.GetComponent<BoxCollider2D>().enabled = false;

                if (waitCounter < 30)
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
                        GameObject.FindObjectOfType<ScoreRecap>().run = true;
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

                //while (nextWarp == previousWarp)
                nextWarp = Random.Range(0, 3);

                previousWarp = nextWarp;

                if (lifePoints <= 0)
                {
                    gameObject.transform.parent = Middle.transform;
                    finalWarp = true;
                    spriteRender.flipX = false;
                    MiddleCollider.SetActive(true);
                }

                else if (nextWarp == 0)
                {
                    gameObject.transform.parent = Left.transform;
                    spriteRender.flipX = true;
                    SkeletonSwordPixelGenerator.facingRight = true;
                }
                else if (nextWarp == 1)
                {
                    gameObject.transform.parent = Right.transform;
                    spriteRender.flipX = false;
                    SkeletonSwordPixelGenerator.facingRight = false;
                }
                else if (nextWarp == 2)
                {
                    gameObject.transform.parent = Middle.transform;
                    spriteRender.flipX = false;
                    SkeletonSwordPixelGenerator.facingRight = false;
                }

                gameObject.transform.localPosition = new Vector3(0, 0, 0);

            }

            if (counter == warpFrames)
            {
                counter = 0;
                gameObject.SetActive(true);
                warping = false;
                spriteRender.sprite = standing;

                if (warpFrames > 20 && warpFrames < 200) //just took damage, so last warp was long. But was not killing blow
                {
                    if (lifePoints >= startingLifePoints / 2)
                        warpFrames = 20;
                    else
                        warpFrames = 10;
                }
            }
        }

        //Standing sprite
        else if (!attackFreeze)
        {
            if (counter == 0)
            {
                spriteRender.sprite = standing;
            }

            if (counter++ == attackAfterFrames)
            {
                counter = 0;
                attackFreeze = true;
            }

        }

        //Attacking sprite (attackFreeze == true)
        else
        {
            attackFreezeCounter = (attackFreezeCounter + 1) % attackFreezeDuration;
            //if (attackFreezeCounter == 0)
            counter = (counter + 1) % (attacking.Length * attackingAnimationSpeed);

            //print("Attack freeze counter is " + attackFreezeCounter);
            //print("Counter is " + counter);

            //Attack after first warp
            if (counter > 0 && previousWarp > -1)
            {
                /*
                //If below 1/2 hp, increase warp speed and only attack after third warp
                if (lifePoints < (startingLifePoints / 2) && warpCounter++ < 2)
                {
                    attackFreezeCounter = 0;
                }*/

                //Above 1/2 hp, normal execution
                //else
                //{
                   // warpCounter = 0;
                	spriteRender.sprite = attacking[counter / attackingAnimationSpeed];
                if (counter / attackingAnimationSpeed == attacking.Length - 8
                   && counter % attackingAnimationSpeed == 0)
                {
                    generateShockWave();
                }

            }

            //Done attacking, warp
            if (counter == 0)
            {
                warping = true;
                attackFreeze = false;
                attackFreezeCounter = 0;
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

    void generateShockWave()
    {
        float x = SkeletonSwordPixelGenerator.facingRight ? 0.8f : -0.8f;
        Vector3 position = new Vector3(x, 0.1f, 0);
        var tempWave = Instantiate(skeletonShockwave, position, Quaternion.identity) as GameObject;

        tempWave.GetComponent<SpriteRenderer>().flipX = !SkeletonSwordPixelGenerator.facingRight;
        tempWave.transform.parent = gameObject.transform;
        tempWave.transform.localPosition = position;

        tempWave.transform.parent = null;
        tempWave.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        tempWave.SetActive(true);
    }

}
