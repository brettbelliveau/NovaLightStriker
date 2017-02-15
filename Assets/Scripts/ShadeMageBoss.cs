using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShadeMageBoss : MonoBehaviour {

    private int counter = 0;
    private int walkingCounter = 0;
    private int attackFreezeCounter = 0;
    
    public int floatInterval = 8;
    private int attackAfterFrames = 60;
    private int warpFrames = 30;
    private int attackFreezeDuration = 60;
    private int previousWarp = -1;
    private int nextWarp = -1;

    private Rigidbody2D body;
    private SpriteRenderer spriteRender;
    private BoxCollider2D collider;
    
    public Sprite floating;
    public Sprite attacking;
    public GameObject topL, topR, botL, botR, gameObject;
    
    private bool takingDamage;
    private bool attackFreeze;
    private bool warping;
    private bool movingRight;
    
    // Use this for initialization
    void Start () {
        body = gameObject.GetComponent<Rigidbody2D>();
        collider = gameObject.GetComponent<BoxCollider2D>();
        spriteRender = gameObject.GetComponent<SpriteRenderer>();
        body = gameObject.GetComponent<Rigidbody2D>();
        attackFreeze = false;
        takingDamage = false;
        movingRight = false;
        warping = false;
    }

    // Update is called once per frame
    void Update() {

        /* Sprite Section */
        Debug.Log("Next " + nextWarp);

        //Talking damage sprite (RESET COUNTER FIRST)
        if (takingDamage)
        {
            //TODO: Taking damage anim + deal with health

            //Done taking damage
            if (counter == 0)
            {
                takingDamage = false;
                transform.rotation = Quaternion.Euler(0, 0, 0);
                GetComponent<Camera>().transform.rotation = Quaternion.Euler(0, 0, 0);
                warping = true;
            }
        }
        
        //Warping to new location
        else if (warping)
        {
            if (counter++ == 0)
            {
                gameObject.SetActive(false);

                while (nextWarp == previousWarp)
                    nextWarp = Random.Range(0,4);

                previousWarp = nextWarp;
                if (nextWarp == 0)
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

                gameObject.transform.localPosition = new Vector3(0,0,0);
            }
            
            if (counter == warpFrames)
            {
                counter = 0;
                gameObject.SetActive(true);
                warping = false;
                spriteRender.sprite = floating;
            }
        }

        //Floating sprite
        else if (!attackFreeze)
        {
            if (counter == 0)
                spriteRender.sprite = floating;

            if (counter++ == attackAfterFrames)
            {
                counter = 0;
                attackFreeze = true;
            }
            else
            {
                //Float Up 1/2 of the time
                if ((counter / (floatInterval) % 2) == 0)
                    gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.GetComponent<Rigidbody2D>().velocity.x, 0.1f);

                //Float Down the other 1/2 of the time
                else
                    gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.GetComponent<Rigidbody2D>().velocity.x, -0.1f);
            }
        }
        

        //Attacking sprite (attackFreeze == true)
        else
        {
            //Stop movement
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            spriteRender.sprite = attacking;
            attackFreezeCounter = (attackFreezeCounter + 1) % attackFreezeDuration;

            //TODO: Throw projectiles everywhere

            if (attackFreezeCounter == 0)
            {
                warping = true;
                attackFreeze = false;
                counter = 0;
            }
        }
        
    }
}
