using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadeKnight : MonoBehaviour {
    
    public int counter = 0;
    private int walkingCounter = 0;
    public int attackFreezeCounter = 0;
    
    public float speed;
    private int turnAfterFrames = 120;
    public int floatSpeed = 10;
    private int attackAnimationSpeed = 3;
    private int attackFreezeDuration = 54;

    private Rigidbody2D body;
    private SpriteRenderer spriteRender;
    private BoxCollider2D collider;
    
    public Sprite moving;
    public Sprite[] attacking;
    public Sprite damageSprite;
    public GameObject orb;
    
    private bool takingDamage;
    private bool attackFreeze;
    private bool movingRight;
    private int damageFrames = 40;
    private int blinkSpeed = 4;

    private int damageCounter = 0;

    // Use this for initialization
    void Start () {
        body = gameObject.GetComponent<Rigidbody2D>();
        collider = gameObject.GetComponent<BoxCollider2D>();
        spriteRender = gameObject.GetComponent<SpriteRenderer>();
        attackFreeze = false;
        takingDamage = false;
        movingRight = false;
    }

    // Update is called once per frame
    void Update() {
       
        /* Sprite Section */

        //Talking damage sprite
        if (takingDamage)
        {
            counter = (counter + 1) % damageFrames;

            //First frame, set sprite and rotate
            if (counter == 1)
            {
                //if facing left, tilt left, else right
                var z = spriteRender.flipX ? -10 : 10;
                transform.rotation = Quaternion.Euler(0, 0, z);
                GetComponent<Camera>().transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            //Every x frames blink character
            if (counter % blinkSpeed * 2 > blinkSpeed)
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
                takingDamage = false;
                transform.rotation = Quaternion.Euler(0, 0, 0);
                GetComponent<Camera>().transform.rotation = Quaternion.Euler(0, 0, 0);
                spriteRender.sprite = moving;
            }
        }

        //Standing sprite
        else if (!attackFreeze)
        {
            walkingCounter = (walkingCounter + 1) % (turnAfterFrames * 2);
            if (walkingCounter == turnAfterFrames)
            {
                movingRight = true;
            }
            else if (walkingCounter == 0)
            {
                movingRight = false;
            }

            //Manual attacking
            else if (walkingCounter == 30 || walkingCounter == 150)
            {
                attackFreeze = true;

                float x = movingRight ? 0.6f : -0.6f;
                Vector3 orbPosition = new Vector3(x, -0.5f, -5);
                var tempOrb = Instantiate(orb, orbPosition, Quaternion.identity) as GameObject;

                tempOrb.transform.parent = gameObject.transform;
                tempOrb.transform.localPosition = orbPosition;

                tempOrb.transform.parent = null;
                tempOrb.SetActive(true);
            }
        }
        

        //Attacking sprite
        else
        {
            if (attackFreezeCounter == 0)
                counter = (counter + 1) % (attacking.Length * attackAnimationSpeed);

            if (counter > 0)
                spriteRender.sprite = attacking[counter / attackAnimationSpeed];

            //Done with first attack frames
            if (counter == 0)
            {
                attackFreezeCounter = (attackFreezeCounter + 1) % attackFreezeDuration;
                if (attackFreezeCounter == 0)
                {
                    attackFreeze = false;
                    spriteRender.sprite = moving;
                }
            }
        }
        
    }

    void FixedUpdate () {

        //Generate left/right movement
        if (!attackFreeze && !takingDamage)
        {
            float movementSpeed = movingRight ? speed : speed * -1; 
            body.velocity = new Vector2(movementSpeed, body.velocity.y);
        }
        else if (attackFreeze)
        {
            body.velocity = new Vector2(0, body.velocity.y);
        }
        
        //Left orientation for sprite if going left, or if standing still and already left
        if (!attackFreeze && !takingDamage)
        {
            spriteRender.flipX = (body.velocity.x > 0) || (spriteRender.flipX && body.velocity.x == 0);
        }
    }
}
