using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterBossRoom : MonoBehaviour {

    public GameObject boss, camera, bossHealthBar;
    private bool startMove;
    private int counter = 0;
    private bool cameraSet;
    private Vector3 position;
    private float originalY;

    // Use this for initialization
    void Start () {
        cameraSet = startMove = false;
	}
	
	// Update is called once per frame
	void Update () {

        if (startMove)
        {
            if (counter++ == 0)
            {
                bossHealthBar.SetActive(true);
                position = new Vector3(camera.transform.position.x, camera.transform.position.y, camera.transform.position.z);
                originalY = position.y;
            }

            //Once boss spawned, script is done
            else if (counter == 60)
            {
                boss.SetActive(true);
            }

            //Float camera up 0.12 units in game space
            if (counter > 60 && !cameraSet)
            {
                position = new Vector3(position.x, position.y + 0.001f, position.z);
                camera.transform.position = position;
                cameraSet = position.y >= originalY+0.1f;
            }

            else if (cameraSet)
            {
                Destroy(gameObject);
                Destroy(this);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //If player enters trigger
        if (other.gameObject.layer == 9)
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            startMove = true;
            Player.stopMovement = true;
        }
    }
}
