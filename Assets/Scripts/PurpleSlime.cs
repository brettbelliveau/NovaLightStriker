using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PurpleSlime : MonoBehaviour {

	public int counter = 0;
	private int walkingCounter = 0;

	public float speed;
	private int startingLifePoints = 10;
	public int lifePoints;
	public int floatInterval = 8;
	private int turnAfterFrames = 120;
	private int attackAnimationSpeed = 30;

	private Rigidbody2D body;
	private SpriteRenderer spriteRender;

	public Sprite[] standing;
	public Sprite[] walking;
	public Sprite[] attacking;
	private List<GameObject> pixels;
	public Sprite damageSprite;
	public GameObject player, pixel;
	public GameObject firstChild, secondChild;

	private bool takingDamage;
	private bool attackFreeze;
	private bool moving;
	private bool movingRight;
	private bool changedDirection;
	private bool startDeleting;
	private float x, y, xV, yV;
	private int index;

	// Use this for initialization
	void Start () {
		body = gameObject.GetComponent<Rigidbody2D>();
		spriteRender = gameObject.GetComponent<SpriteRenderer>();
		pixels = new List<GameObject>();
		attackFreeze = false;
		takingDamage = false;
		moving = false;
		movingRight = false;
		changedDirection = false;

		lifePoints = startingLifePoints;
//		spawnBunchOfPixels (100); // spawn in, not out
	}

	// Update is called once per frame
	void Update() {
		gameObject.GetComponent<Rigidbody2D>().gravityScale = 4f;
		Physics2D.IgnoreLayerCollision(12, 9, false);
		Physics2D.IgnoreLayerCollision(14, 11, true); // ignore platform and enemy collision
		// Player is near. Close in terms of x axis
		// start to jump towards player
		if (takingDamage) {
			counter = (counter + 1) % 40;
			if (counter == 1 && !startDeleting)
			{
				body.velocity = new Vector2(0, 0);
				spriteRender.sprite = null;

				//Increase velocity depending on dist. to player
				x = gameObject.transform.position.x - player.transform.position.x;
				y = gameObject.transform.position.y - player.transform.position.y + 0.137f;

				xV = -0.1f * (x * x);
				yV = 0.3f * (y * y);

				int r = 0;
				for (int i = 0; i < 10; i++)
				{
					for (int j = 0; j < 30; j++)
					{
						r = Random.Range(0, 3);
						if (r == 0) {
							pixels.Add(spawnPixelAtFixedLocation(pixel, i, j));
						}
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
						index = pixels.Count - 1;
						if (index >= 0)
						{
							Destroy(pixels[index]);
							pixels.RemoveAt(index);
						}
					}
				}
				else
				{
					Destroy(gameObject);
					Destroy(this);
				}
			}
		}
		else if (playerIsNear ()) {
			counter = (counter + 1) % (attacking.Length * attackAnimationSpeed);
			spriteRender.sprite = attacking [counter / attackAnimationSpeed];

			if (counter / attackAnimationSpeed < 2) {
				moving = false;
				gameObject.GetComponent<Rigidbody2D> ().velocity = new Vector2 (0, 0);
//					return;
			} else {
				if (System.Math.Abs(x) < 0.01f) {
					moving = false;
				} else {
					moving = true;
					if (x > -2.4f && x < 0) {
						if (movingRight == false) {
							changedDirection = true;
						}
						movingRight = true;
					}
					//Player is on the left
					else if (x < 2.4f && x > 0) {
						if (movingRight == true) {
							changedDirection = true;
						}
						movingRight = false;
					}
				}
				//Jump 1/2 of the time
				if (counter /attackAnimationSpeed == 2) {
					Physics2D.IgnoreLayerCollision(12, 11, true);
					gameObject.GetComponent<Rigidbody2D>().gravityScale = 0.0f;
					if (moving == true) {
						gameObject.GetComponent<Rigidbody2D> ().velocity = new Vector2 (gameObject.GetComponent<Rigidbody2D> ().velocity.x, 0.7f);
					} else {
						gameObject.GetComponent<Rigidbody2D> ().velocity = new Vector2 (0, 0.7f);
					}
				}

				//Float Down the other 1/2 of the time
				else if (counter /attackAnimationSpeed == 3) {
					gameObject.GetComponent<Rigidbody2D>().gravityScale = 4f;
					Physics2D.IgnoreLayerCollision(12, 11, false); // un-ignore platform and projectile collision
					Physics2D.IgnoreLayerCollision(12, 9, true);
					if (moving == true) {
						gameObject.GetComponent<Rigidbody2D> ().velocity = new Vector2 (gameObject.GetComponent<Rigidbody2D> ().velocity.x, -0.7f);
					} else {
						gameObject.GetComponent<Rigidbody2D> ().velocity = new Vector2 (0, -0.7f);
					}
				}
			}
		}

		walkingCounter = (walkingCounter + 1) % (turnAfterFrames * 2);
	}

	void FixedUpdate () {
		var scale = transform.localScale.x;
//		Generate left/right movement
		if (!attackFreeze && !takingDamage && moving)
		{
			float movementSpeed = movingRight ? speed : speed * -1; 
			gameObject.GetComponent<Rigidbody2D> ().velocity = new Vector2(movementSpeed, body.velocity.y);
		}

		//Left orientation for sprite if going right, or if standing still and already right
		if (!attackFreeze && !takingDamage && moving)
		{
			if (changedDirection) {
				Vector3 rot = transform.rotation.eulerAngles;
				rot = new Vector3 (rot.x, rot.y + 180, rot.z);
				transform.rotation = Quaternion.Euler (rot);
				changedDirection = false;
			}
		}
	}

	private bool playerIsNear()
	{
		//Calculate distance to player from body
 		x = gameObject.transform.position.x - player.transform.position.x;
		y = gameObject.transform.position.y - player.transform.position.y;

		//Player is on the right and enemy unit is facing right
		if (x > -2.4f && x < 0)
			return true;

		//Player is on the left and enemy unit is facing left
		else if (x < 2.4f && x > 0)
			return true;

		else
			return false;
	}

	public void sendDamage(int damage)
	{
		var tempLifePoints = lifePoints - damage;
		lifePoints = tempLifePoints < 0 ? 0 : tempLifePoints;

		BossHealthBar.changed = true;

		if (lifePoints == 0) {
			takingDamage = true;
			splitInTwo ();
		}
	}

	private void splitInTwo(){
		// need to spawn in the two with pixels!
		if ((firstChild != null) && (secondChild != null)) {
			x = gameObject.transform.position.x - player.transform.position.x;
	
			firstChild.SetActive (true);
			secondChild.SetActive (true);

			//Player is on the right
			if (x < -2.4f) {
				firstChild.transform.position = new Vector2 (transform.position.x - .4f, transform.position.y - (transform.position.y * .082f));
				secondChild.transform.position = new Vector2 (transform.position.x - .3f, transform.position.y - (transform.position.y * .082f));
			}

		//Player is on the left
		else {
				firstChild.transform.position = new Vector2 (transform.position.x + .4f, transform.position.y - (transform.position.y * .082f));
				secondChild.transform.position = new Vector2 (transform.position.x + .3f, transform.position.y - (transform.position.y * .082f));
			}

	
//		secondChild.transform.position = transform.position;
			var slime = secondChild.GetComponent<PurpleSlime> ();
			slime.attackAnimationSpeed = 26;
		}
	}

	private void spawnBunchOfPixels(int num)
	{
		for (int i = 0; i < num; i++)
			pixels.Add(spawnPixelAtRandomLocation(pixel));
	}

	private GameObject spawnPixelAtRandomLocation(GameObject pixel)
	{
		var location = Vector3.zero;
		var collider = gameObject.GetComponent<BoxCollider2D> ();
		location.x = Random.Range(collider.offset.x - (collider.size.x / 2f), collider.offset.x + (collider.size.x /2f));
		location.y = Random.Range(-1.5f, 1f);
		location.z = 2;

		return (spawnPixelAtLocation(pixel, location));
	}

	private GameObject spawnPixelAtFixedLocation(GameObject pixel, int x, int y)
	{
		var location = Vector3.zero;

		location.x = 0.075f * x - 0.425f;
		location.y = 0.07f * y - 1.1f;
		location.z = Random.Range(2f, 3f);

		return (spawnPixelAtLocation(pixel, location));
	}

	private GameObject spawnPixelAtLocation(GameObject pixel, Vector3 location)
	{
		var tempPixel = Instantiate(pixel, location, Quaternion.identity) as GameObject;

		tempPixel.transform.parent = gameObject.transform;
		tempPixel.transform.localPosition = location;

		tempPixel.transform.parent = null;
		tempPixel.SetActive(true);

		var xVelocity = Random.Range(xV - 0.08f, xV + 0.08f);
		var yVelocity = Random.Range(yV, yV + 0.1f);

		tempPixel.GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity, yVelocity);
		tempPixel.GetComponent<Rigidbody2D>().gravityScale = Random.Range(-0.035f, 0.02f);

		return tempPixel;
	}
}
