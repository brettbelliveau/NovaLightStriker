using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PurpleSlime : MonoBehaviour {

	private int counter = 0;
	private int walkingCounter = 0;
	private int attackFreezeCounter = 0;

	public float speed;
	public int floatInterval = 8;
	private int turnAfterFrames = 120;
	private int attackAnimationSpeed = 3;
	private int standAnimationSpeed = 30;
	private int walkAnimationSpeed = 30;
	private int attackFreezeDuration = 54;

	private Rigidbody2D body;
	private SpriteRenderer spriteRender;
	private BoxCollider2D collider;

	public Sprite[] standing;
	public Sprite[] walking;
	public Sprite[] attacking;
	public Sprite damageSprite;
	public GameObject orb;

	private bool takingDamage;
	private bool attackFreeze;
	private bool moving;
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
		moving = false;
		movingRight = false;
	}

	// Update is called once per frame
	void Update() {

		/* Sprite Section */

		//Talking damage sprite
		if (takingDamage)
		{
//			counter = (counter + 1) % damageFrames;
//
//			//TODO: Taking damage anim
//
//			//Done taking damage
//			if (counter == 0)
//			{
//				takingDamage = false;
//				transform.rotation = Quaternion.Euler(0, 0, 0);
//				GetComponent<Camera>().transform.rotation = Quaternion.Euler(0, 0, 0);
//				spriteRender.sprite = moving;
//			}
		}

		//Standing sprite
		else if (!attackFreeze)
		{
			// Half the time stand in place
			if (walkingCounter < (turnAfterFrames)) {
				moving = false;
				gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
				counter = (counter + 1) % (standing.Length * standAnimationSpeed);
				spriteRender.sprite = standing [counter / standAnimationSpeed];
			} else {
				moving = true;
				counter = (counter + 1) % (walking.Length * standAnimationSpeed);
				spriteRender.sprite = walking [counter / standAnimationSpeed];
				if (walkingCounter < (2 * turnAfterFrames * (.75))) {
					movingRight = true;
				} else {
					movingRight = false;
				}

				//Float Up 1/2 of the time
				if ((walkingCounter / (turnAfterFrames*2 / floatInterval) % 2) == 0)
					gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.GetComponent<Rigidbody2D>().velocity.x, 0.04f);

				//Float Down the other 1/2 of the time
				else
					gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(gameObject.GetComponent<Rigidbody2D>().velocity.x, -0.04f);
				
				// jump around
			}

			walkingCounter = (walkingCounter + 1) % (turnAfterFrames * 2);

//			walkingCounter = (walkingCounter + 1) % (turnAfterFrames * 2);
//			if (walkingCounter == turnAfterFrames)
//			{
//				movingRight = true;
//			}
//			else if (walkingCounter == 0)
//			{
//				movingRight = false;
//			}
//
//			//Manual attacking
//			else if (walkingCounter == 150)
//			{
//				attackFreeze = true;
//
//				float x = movingRight ? 0.6f : -0.6f;
//				Vector3 orbPosition = new Vector3(x, -0.5f, -5);
//				var tempOrb = Instantiate(orb, orbPosition, Quaternion.identity) as GameObject;
//
//				tempOrb.transform.parent = gameObject.transform;
//				tempOrb.transform.localPosition = orbPosition;
//
//				tempOrb.transform.parent = null;
//				tempOrb.SetActive(true);
//			}
		}
//
//
//		//Attacking sprite
//		else
//		{
//			//Stop movement
//			gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
//
//			if (attackFreezeCounter == 0)
//				counter = (counter + 1) % (attacking.Length * attackAnimationSpeed);
//
//			if (counter > 0)
//				spriteRender.sprite = attacking[counter / attackAnimationSpeed];
//
//			//Done with first attack frames
//			if (counter == 0)
//			{
//				attackFreezeCounter = (attackFreezeCounter + 1) % attackFreezeDuration;
//				if (attackFreezeCounter == 0)
//				{
//					attackFreeze = false;
//					spriteRender.sprite = moving;
//				}
//			}
//		}

	}

	void FixedUpdate () {

//		Generate left/right movement
		if (!attackFreeze && !takingDamage && moving)
		{
			float movementSpeed = movingRight ? speed : speed * -1; 
			body.velocity = new Vector2(movementSpeed, body.velocity.y);
		}
		else if (attackFreeze)
		{
			body.velocity = new Vector2(0, body.velocity.y);
		}

		//Left orientation for sprite if going right, or if standing still and already right
		if (!attackFreeze && !takingDamage && moving)
		{
			spriteRender.flipX = (body.velocity.x > 0) || (spriteRender.flipX && body.velocity.x == 0);
		}
	}
}
