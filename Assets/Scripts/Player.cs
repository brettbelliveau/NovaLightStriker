using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public float speed;
    public static int currentLevel;         
    public static int lifePoints;
    public static int score;                    
    public static int totalScore;           
    public static int multiplier;
    public static int extraLives;      
    private static float lastKillTime;
    public static float hyperActiveTime;
    private static bool turnOnHyperMode = false;
    private static bool enterBossFight;
    public bool onGround;
    public static int counter = 0;
    public static int pixelCounter = 0;
    public int runAnimationSpeed = 3;
    public static int attackAnimationSpeed = 2;
    private static int spawnAnimationSpeed = 8;
    private int framesSinceOnGround = 0;
    private bool jumped = false;
    public static List<bool> hitFromLeft;
    public static List<int> lastDamageTaken;
    private static int waitFrames = 40; //number of frames to wait before spawning
    
    private Rigidbody2D body;
    private SpriteRenderer spriteRender;
    private BoxCollider2D collider;
    private BoxCollider2D swordColliderObject;
    public GameObject camera, shockwave, pixel;
    public GameObject healthBarObject, energyBarObject;
    public GameObject scoreText, totalScoreText, multiplierText;
    private Slider healthBar, energyBar;
    private Vector2 defaultMultScale;

    public Sprite standing, jumping;
    public Sprite[] running;
    public Sprite[] attacking;
    public Sprite[] spawning;
    public Sprite damageSprite;
    public GameObject damageColliderRight, damageColliderLeft;
    public GameObject swordCollider;
    public GameObject lifeCounter, lifeCounterText;
    public GameObject musicSource;
    public AudioClip dying;

    public static bool spawningBool; //Should be true to init spawn anim
    public static bool spawned;      //Should be false to init spawn anim

    public static bool attackFreeze;
    public static bool takingDamage;
    private int damageFrames = 20;
    private int blinkSpeed = 5;
    public static bool hyperModeActive = false;  //Should be false at start
    private bool disableFrontCollider;
    private bool disableBackCollider;
    public static bool invincibleFrames;
    private int iCounter = 0;
    private bool spriteEnabled = true;
    private bool startDeleting = false;
    private List<GameObject> pixels;
    private float xV, yV;
    private bool earlySwing = false;
    public static bool bossDefeated = false;
    private static bool multiplierReset = false;
    private int maxLifePoints;
    private float energyBarValue;
    private bool lifeCounterOn;
    private bool movedUp;
    public static bool stopMovement;
    public static float awakeTime, finishTime, timeLost, timeAtCheckPoint;
    private bool calledDyingAnim;
    private bool playedGameOverText;
    public static bool checkPointOne, checkPointTwo;
    public GameObject checkPointOneObject, checkPointTwoObject, startingPosition;
    private float startTime;
    private float volume;
    public static bool fadeOutSound;


    // Use this for initialization
    void Start () {
		gameObject.tag = "Entity";
        checkPointOne = ("True".Equals(PlayerPrefs.GetString("CheckPointOne")));
        checkPointTwo = ("True".Equals(PlayerPrefs.GetString("CheckPointTwo")));
        timeLost = PlayerPrefs.GetFloat("TimeLost");

        if (PlayerPrefs.GetInt("PreviousLevel") == 0 && timeLost == 0)
            PlayerPrefs.SetInt("ExtraLives", 2);

        currentLevel = PlayerPrefs.GetInt("CurrentLevel");

        //If reached no checkpoints and haven't died yet
        if (!checkPointOne && !checkPointTwo && timeLost == 0)
        {
            string writeText;

            Debug.Log("CurrentLevel " + currentLevel);

            if (currentLevel == 1)
                writeText = "Level One";

            else if (currentLevel == 2)
                writeText = "Level Two";

            else
                writeText = "Level Three";

            GameObject.FindObjectOfType<TextController>().writeText(writeText, 40, 100, 14);
            stopMovement = true;
        }
        
        //Reached checkpoint two
        if (checkPointTwo)
        {
            transform.position = checkPointTwoObject.transform.position;
            awakeTime = PlayerPrefs.GetFloat("AwakeTime");
            score = PlayerPrefs.GetInt("Score");
            timeAtCheckPoint = Time.time;
        }
        //Reached checkpoint one
        else if (checkPointOne)
        {
            transform.position = checkPointOneObject.transform.position;
            awakeTime = PlayerPrefs.GetFloat("AwakeTime");
            score = PlayerPrefs.GetInt("Score");
            timeAtCheckPoint = Time.time;
        }
        //No checkpoints reached
        else
        {
            transform.position = startingPosition.transform.position;
            awakeTime = Time.time;
            score = 0;
            timeAtCheckPoint = 0;
        }

        //Already died in level
        if (timeLost > 0)
        {
            spawningBool = false;
            spawned = true;
            extraLives = PlayerPrefs.GetInt("ExtraLives");
        }
        else
        {
            spawningBool = true;
            spawned = false;
            extraLives = 2;
        }

        currentLevel = PlayerPrefs.GetInt("CurrentLevel");
        
        if (currentLevel < 1)
        {
            currentLevel = 1;
            PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        }
        else
        {
            extraLives = PlayerPrefs.GetInt("ExtraLives");
            totalScore = PlayerPrefs.GetInt("TotalScore");
        }

        lifePoints = 100;
        maxLifePoints = lifePoints;
        lastKillTime = 0;
        multiplier = 1;

        body = gameObject.GetComponent<Rigidbody2D>();
        collider = gameObject.GetComponent<BoxCollider2D>();
        spriteRender = gameObject.GetComponent<SpriteRenderer>();
        pixels = new List<GameObject>();
        lastDamageTaken = new List<int>();

        attackFreeze = false;
        takingDamage = false;
        counter = 0;
        spriteRender.sprite = null;
        SpawnInOutPixels.spawning = spawningBool;
        SpawnInOutPixels.spawned = spawned;

        swordColliderObject = swordCollider.gameObject.GetComponent<BoxCollider2D>();

        hitFromLeft = new List<bool>();
        
        //Ignore Layer collision for sword (15) and all non-enemy or projectile layers
        Physics2D.IgnoreLayerCollision(15, 0, true);
        Physics2D.IgnoreLayerCollision(15, 5, true);
        Physics2D.IgnoreLayerCollision(15, 9, true);
        Physics2D.IgnoreLayerCollision(15, 10, true);
        Physics2D.IgnoreLayerCollision(15, 11, true);
        Physics2D.IgnoreLayerCollision(15, 13, true);
        Physics2D.IgnoreLayerCollision(15, 16, true);

        //Ignore Layer collision for enemy projectiles and themselves
        Physics2D.IgnoreLayerCollision(12, 12, true);

        //Ignore Layer collision for projectile (12) and enemy (14) layers
        Physics2D.IgnoreLayerCollision(12, 14, true);

        //Ignore Layer collision for projectile (12) and health item (15) layers
        Physics2D.IgnoreLayerCollision(12, 16, true);
        
        //UI Element initialization
        defaultMultScale = multiplierText.transform.localScale;

        healthBar = healthBarObject.GetComponent<Slider>();
        energyBar = energyBarObject.GetComponent<Slider>();
        energyBar.value = 0;

        lifeCounterText.GetComponent<Text>().text = "x" + extraLives;
        lifeCounterOn = true;
        movedUp = false;
        startTime = Time.time;
        bossDefeated = false;
        volume = 0;
        fadeOutSound = false;
    }

    // Update is called once per frame
    void Update() {

        if (musicSource == null)
        {
            musicSource = FindObjectOfType<AudioScript>().gameObject;
            volume = musicSource.GetComponent<AudioSource>().volume;
        }

        if (!musicSource.GetComponent<AudioSource>().isPlaying)
        {
            musicSource.GetComponent<AudioSource>().Play();
        }

        if (volume < .12f && !fadeOutSound)
        {
            volume += 0.001f;
            musicSource.GetComponent<AudioSource>().volume = volume;
        }

        else if (fadeOutSound)
        {
            volume -= 0.001f;
            musicSource.GetComponent<AudioSource>().volume = volume;
        }



        if (Time.timeScale != 1)
            return;
        
        //Turn off life counter after 4 seconds
        if (lifeCounterOn)
        {
            //Move up
            if (Time.time - startTime > 0.5 && lifeCounter.transform.position.y < 0 && !movedUp)
                lifeCounter.transform.position = new Vector2(lifeCounter.transform.position.x, lifeCounter.transform.position.y + 2f);

            //Move down
            else if (Time.time - startTime > 3.5 && lifeCounter.transform.position.y > -100)
            {
                movedUp = true;
                lifeCounter.transform.position = new Vector2(lifeCounter.transform.position.x, lifeCounter.transform.position.y - 2f);
            }

            else if (Time.time - startTime > 10)
            {
                lifeCounterOn = false;
                lifeCounter.SetActive(false);
            }
        }
    
        healthBar.value = ((float)lifePoints / (float)maxLifePoints);

        //If in hyper mode, represent energy bar as remaining energy
        //If not, represent it as multiplier / max multiplier
        if (multiplier == 8) {
            energyBar.value = ((hyperActiveTime + 10000) - Time.time * 1000) / 10000;
        }
        else
            energyBar.value = (float)(multiplier - 1) / 7f;
      
        scoreText.GetComponent<Text>().text = score.ToString("D5");
        totalScoreText.GetComponent<Text>().text = totalScore.ToString("D6");
        multiplierText.GetComponent<Text>().text = "x" + multiplier.ToString();

        //Reset size of multiplier text when it is changed by static call
        if (multiplierReset)
        {
            multiplierReset = false;
            multiplierText.transform.localScale = defaultMultScale;
        }
            
        //Reduce size of multiplier text over time
        if (multiplier > 1 && !hyperModeActive && multiplier < 8)
        {   
            var x = multiplierText.transform.localScale.x;
            var y = multiplierText.transform.localScale.y;
            multiplierText.transform.localScale = new Vector2(x * 0.997f, y * 0.997f);
        }

        //If hyper mode on and not in boss fight
        if (hyperModeActive && !enterBossFight)
        {
            //Turn off after 10 seconds
            if (Time.time * 1000 > hyperActiveTime + 9900)
            {
                hyperModeActive = false;
                multiplier = 1;
                multiplierText.GetComponent<Text>().text = "x" + multiplier.ToString();
                multiplierReset = true;
                multiplierText.transform.localScale = defaultMultScale;

                EnergyBar.alternate = false;
                EnergyBar.backToNormal = true;
            }
        }

        //If not in hyper mode and mult > 1 AND 3 seconds since last kill
        else if (multiplier > 1 && Time.time*1000 > lastKillTime + 3000)
        {
            multiplier = 1;
            multiplierText.transform.localScale = defaultMultScale;
            multiplierText.GetComponent<Text>().text = "x" + multiplier.ToString();
        }

        if (invincibleFrames)
        {
            iCounter = (iCounter + 1) % 50;

            if (iCounter % blinkSpeed == 0)
                spriteEnabled = !spriteEnabled;

            spriteRender.enabled = spriteEnabled;

            if (iCounter == 0)
            {
                invincibleFrames = false;
                spriteRender.enabled = true;
                spriteEnabled = true;
            }
            
        }

        if (!spawningBool && spawned)
        {
            //Check if back on ground after jump/fall
            if (!onGround && Mathf.Abs(body.velocity.y) < 0.02) {
                onGround = true;
                framesSinceOnGround = 0;
                jumped = false;
            }

            //Turn off onGround when falling from ledge
            if (onGround)
                onGround = Mathf.Abs(body.velocity.y) < 0.02;
            //Use this counter as a check for how long since fallen off ledge
            else
                framesSinceOnGround++;

            //Give the user 2 frames after leaving platform to jump
            if (Input.GetButtonDown("Jump") && (onGround || (!jumped && framesSinceOnGround < 2)) && !attackFreeze && !takingDamage && !bossDefeated && !stopMovement)
            {
                body.AddForce(Vector2.up * 14);
                onGround = false;
                jumped = true;
            }

            if (Input.GetButtonDown("Fire1") && !attackFreeze && !takingDamage && !bossDefeated && !stopMovement)
            {
                counter = 0;
                attackFreeze = true;
                SwordPixelGenerator.attacking = true;
            }
        }

        /* Sprite Section */

        //Talking damage sprite
        if (takingDamage)
        {
            if (lifePoints <= 0)
            {
                playDyingAnimation();
            }
            else
            {
                counter = (counter + 1) % damageFrames;

                if (counter == 1)
                {
                    earlySwing = true;
                    disableFrontCollider = disableBackCollider = false;

                    lifePoints -= lastDamageTaken[0];
                    HealthBar.changed = true;
                    lastDamageTaken.Clear();

                    if (lifePoints <= 0)
                    {
                        playDyingAnimation();
                    }

                    else
                    {
                        swordCollider.gameObject.SetActive(false);
                        attackFreeze = SwordPixelGenerator.attacking = false;

                        //If moving up, push down.
                        var y = body.velocity.y > 0.02 ? -0.02f : 0f;
                        body.velocity = new Vector2(0, y);

                        hitFromLeft.Clear();
                        //if facing left, tilt left, else right
                    }
                }

                //Every x frames blink character
                if (counter % blinkSpeed == 0)
                {
                    spriteRender.sprite = damageSprite;
                }
                else
                {
                    spriteRender.sprite = null;
                }

                //Done taking damage
                if (counter == 0)
                {
                    lastDamageTaken.Clear();
                    invincibleFrames = true;
                    takingDamage = false;
                }
            }
        }
        //Standing sprite
        else if (!attackFreeze && body.velocity.x == 0 && Mathf.Abs(body.velocity.y) < 0.02 && !spawningBool && spawned)
        {
            if (stopMovement && spriteRender.sprite == jumping)
                spriteRender.sprite = jumping;
            else
                spriteRender.sprite = standing;
        }

        //Running sprite
        else if (!attackFreeze && body.velocity.x != 0 && Mathf.Abs(body.velocity.y) < 0.02 && !spawningBool && spawned)
        {
            counter = (counter + 1) % (running.Length * runAnimationSpeed);
            spriteRender.sprite = running[counter / runAnimationSpeed];
        }

        //Attacking sprite
        else if (attackFreeze)
        {
            counter = (counter + 1) % (attacking.Length * attackAnimationSpeed);
            spriteRender.sprite = attacking[counter / attackAnimationSpeed];

            //After 4 frames, invincible from front and back
            if (counter == 4)
            {
                disableBackCollider = true;
                disableFrontCollider = true;
            }
            
            //After 5th sprite frame, not invincible from back
            if (counter / attackAnimationSpeed == 5)
                disableBackCollider = false;

            //After 4th sprite frame, turn on sword collider 
            if (counter / attackAnimationSpeed == 4)
            {
                swordCollider.gameObject.SetActive(true);
                gameObject.GetComponent<AudioSource>().Play();
                earlySwing = true;
            }

            //After 9th sprite frame, move sword collider away from character
            else if (counter / attackAnimationSpeed == 9)
            {
                earlySwing = false;
            }

            //After 12th sprite frame, turn off sword collider 
            else if (counter / attackAnimationSpeed == 12)
            {
                swordCollider.gameObject.SetActive(false);
                earlySwing = true;
            }

            //Around when the sword slash anim has finished
            //If in hyper mode
            //Sword slash inits shockwave
            if (hyperModeActive)
            {
                if (counter / attackAnimationSpeed == attacking.Length - 8
                    && counter % attackAnimationSpeed == 0)
                    generateShockWave();
            }

            //Done attacking
            if (counter == 0)
            {
                attackFreeze = SwordPixelGenerator.attacking = false;
                disableFrontCollider = false;
                damageColliderLeft.SetActive(true);
                damageColliderRight.SetActive(true);
                if (turnOnHyperMode)
                {
                    turnOnHyperMode = false;
                    hyperModeActive = true;
                }
            }
        }

        else if (spawningBool)
        {
            if (!spawned) //spawn-in animation
            {
                if (counter < waitFrames)
                {
                    counter++;
                    if (counter >= waitFrames)
                    {
                        waitFrames = -1;
                        counter = 0;
                    }
                }
                else
                {
                    counter = (counter + 1) % (spawning.Length * spawnAnimationSpeed);
                    spriteRender.sprite = spawning[counter / spawnAnimationSpeed];
                    //Done spawning
                    if (counter == 0)
                    {
                        spriteRender.sprite = standing;
                        spawningBool = false;               
                        spawned = true;
                        SpawnInOutPixels.spawning = false;  
                        SpawnInOutPixels.spawned = true;
                    }
                }
            }
            else //spawn-out animation
            {
                counter = (counter + 1) % (spawning.Length * spawnAnimationSpeed);
                if (counter > 0)
                    spriteRender.sprite = spawning[spawning.Length - (counter / spawnAnimationSpeed) - 1];
                //Done spawning out
                if (counter == 0)
                {
                    spriteRender.sprite = null;
                    spawningBool = false;
                    spawned = false;
                    SpawnInOutPixels.spawning = false;
                    SpawnInOutPixels.spawned = false;
                }
            }
        }

        //Jumping sprite
        else if (body.velocity.y != 0)
        {
            spriteRender.sprite = jumping;
        }
        
        if (disableFrontCollider)
        {
            if (!spriteRender.flipX)
            {
                damageColliderRight.SetActive(false);
                damageColliderLeft.SetActive(!disableBackCollider);
            }
            else
            {
                damageColliderLeft.SetActive(false);
                damageColliderRight.SetActive(!disableBackCollider);
            }
        }

        if (earlySwing)
        {
            swordColliderObject.offset = (spriteRender.flipX) ? new Vector2(0f, -0.05f) : new Vector2(0f, -0.05f);
            swordColliderObject.size = new Vector2(0.3f, 0.7f);
        }
        else
        {
            swordColliderObject.offset = (spriteRender.flipX) ? new Vector2(-0.65f, -0.05f) : new Vector2(0.65f, -0.05f);
            swordColliderObject.size = new Vector2(0.3f, 1.8f);
        }
    }

    void FixedUpdate () {

        //Obtain left/right input
        if (!spawningBool && spawned && !takingDamage && !bossDefeated && !stopMovement)
        {
            body.gravityScale = 0.25f;
            float h = Input.GetAxisRaw("Horizontal");
            body.velocity = new Vector2(speed * h, body.velocity.y);
        }

        else if (bossDefeated)
        {
            body.velocity = new Vector2(0, body.velocity.y);
            if (body.velocity.y < 0.02f)
                body.gravityScale = 0.01f;
            else if (body.velocity.y < -0.1f)
                body.gravityScale = 0;
        }
        
        else if (stopMovement)
        {
            body.velocity = new Vector2(0, 0);
            body.gravityScale = 0f;
        }
       

        //Turn off collider when moving upward
        //Layer 9 is character, layer 11 is special platforms
        Physics2D.IgnoreLayerCollision(9, 11, body.velocity.y >= 0.02);

        //Left orientation for sprite if going left, or if standing still and already left
        if (!takingDamage && !spawningBool && spawned)
        {
            spriteRender.flipX = (body.velocity.x < 0) || (spriteRender.flipX && body.velocity.x == 0);

            SwordPixelGenerator.facingRight = !spriteRender.flipX;
            ShockWave.facingRight = spriteRender.flipX;
        }
    }

    void generateShockWave()
    {
        float x = SwordPixelGenerator.facingRight ? 0.8f : -0.8f;
        Vector3 position = new Vector3(x, 0.1f, 0);
        var tempWave = Instantiate(shockwave, position, Quaternion.identity) as GameObject;

        tempWave.GetComponent<SpriteRenderer>().flipX = !SwordPixelGenerator.facingRight;
        tempWave.transform.parent = gameObject.transform;
        tempWave.transform.localPosition = position;

        tempWave.transform.parent = null;
        tempWave.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        tempWave.SetActive(true);
    }

    public void playDyingAnimation()
    {
        //TODO: Make this only play when out of lives
        if (extraLives == 0 && !playedGameOverText)
        {
            playedGameOverText = true;
            GameObject.FindObjectOfType<TextController>().writeText("Game Over", 40, 2000, 10);
        }
        
        counter = (counter + 1) % 80;
        if (counter == 2 && !startDeleting)
        {
            gameObject.GetComponent<AudioSource>().clip = dying;
            gameObject.GetComponent<AudioSource>().Play();
            body.velocity = new Vector3(0, 0, 0);
            body.gravityScale = 0;
            collider.enabled = false;
            damageColliderLeft.SetActive(false);
            damageColliderRight.SetActive(false);
            swordCollider.SetActive(false);
            spriteRender.sprite = null;

            xV = 0;
            yV = 0.1f;
            
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                    pixels.Add(spawnPixelAtFixedLocation(pixel, i, j));
                    pixels.Add(spawnPixelAtFixedLocation(pixel, i, j));
                }
            }
        }
        else if (counter == 0 || startDeleting)
        {
            startDeleting = true;
            if (pixels.Count > 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    int index = pixels.Count - 1;
                    if (index >= 0)
                    {
                        Destroy(pixels[index]);
                        pixels.RemoveAt(index);
                    }
                }
            }
            else if (pixels.Count == 0 && !calledDyingAnim)
            {
                calledDyingAnim = true;
                //If out of lives, return to main menu
                if (extraLives == 0)
                {
                    fadeOutSound = true;
                    PlayerPrefs.DeleteAll();
                    PlayerPrefs.SetInt("CurrentLevel", currentLevel);
                    GameObject.FindObjectOfType<ScreenFader>().speed = 0.8f;
                    GameObject.FindObjectOfType<ScreenFader>().EndScene(0);
                }
                else
                {
                    if (checkPointOne || checkPointTwo)
                        timeLost += Time.time - timeAtCheckPoint;
                    else
                        timeLost += Time.time - awakeTime;

                    PlayerPrefs.SetInt("ExtraLives", extraLives-1);
                    PlayerPrefs.SetInt("CurrentLevel", currentLevel);
                    PlayerPrefs.SetString("CheckPointOne", checkPointOne ? "True" : "False");
                    PlayerPrefs.SetString("CheckPointTwo", checkPointTwo ? "True" : "False");
                    PlayerPrefs.SetFloat("AwakeTime", awakeTime);
                    PlayerPrefs.SetFloat("TimeLost", timeLost);
                    GameObject.FindObjectOfType<ScreenFader>().speed = 1.5f;
                    GameObject.FindObjectOfType<ScreenFader>().EndScene(currentLevel);
                }
            }
        }
        else
            body.velocity = new Vector3(0, 0, 0);
    }

    private GameObject spawnPixelAtFixedLocation(GameObject pixel, int x, int y)
    {
        var location = Vector3.zero;

        location.x = 0.075f * x - 0.425f;
        location.y = 0.07f * y - 1.1f;
        location.z = 0f;

        return (spawnPixelAtLocation(pixel, location));
    }

    private GameObject spawnPixelAtLocation(GameObject pixel, Vector3 location)
    {
        var tempPixel = Instantiate(pixel, location, Quaternion.identity) as GameObject;

        tempPixel.transform.parent = gameObject.transform;
        tempPixel.transform.localPosition = location;

        tempPixel.transform.parent = null;
        tempPixel.SetActive(true);

        var xVelocity = Random.Range(-0.25f, 0.25f);
        var yVelocity = Random.Range(-0.25f, 0.35f);

        tempPixel.GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity, yVelocity);
        tempPixel.GetComponent<Rigidbody2D>().gravityScale = 0;

        return tempPixel;
    }

    public static void addLifePoints(int points)
    {
        lifePoints += points;
        HealthBar.changed = true;
        lifePoints = lifePoints > 100 ? 100 : lifePoints;
    }

    public static void addScorePoints(int points)
    {
        score += points * multiplier;
        totalScore += points * multiplier;

        //Boss kill, dont touch multiplier
        if (points >= 10000)
            return;

        //If not in hyper mode, increase multiplier
        if (!hyperModeActive && multiplier < 8)
        {
            multiplier++;
            multiplier = multiplier > 8 ? 8 : multiplier;
            multiplierReset = true;
        }
        
        if (multiplier == 8 && !hyperModeActive)
            initHyperMode();
        
        lastKillTime = Time.time * 1000;
    }

    private static void initHyperMode()
    {
        turnOnHyperMode = true;
        hyperActiveTime = Time.time * 1000;
        EnergyBar.alternate = true;
        //TODO: Start up some vfx or visual cue
    }
}
