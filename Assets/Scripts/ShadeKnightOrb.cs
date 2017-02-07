using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadeKnightOrb : MonoBehaviour {

    public GameObject shadeKnight;
    public Sprite[] sprites;
    public float speed;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D body;
    private bool movingRight;

    private int counter = 0;
    
	// Use this for initialization
	void Start () {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        body = gameObject.GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {
        movingRight = shadeKnight.GetComponent<SpriteRenderer>().flipX;

        counter++;
        //Charging up orb
        if (counter < 72)
        {
            if (counter <= 60)
                spriteRenderer.sprite = sprites[counter/12];
        }
        //Firing Orb
        else if (counter == 72)
        {
            Vector2 velocity = movingRight ? new Vector2(speed, 0) : new Vector2(-1*speed,0);
            body.velocity = velocity;
        }
        //Else if orb flying too long, delete
        else if (counter > 300)
        {
            Destroy(gameObject);
            Destroy(this);
        }
        //TODO: Add delete on collision
    }
}
