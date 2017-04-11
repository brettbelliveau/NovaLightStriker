using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBoss : MonoBehaviour {
	
	public GameObject[] children;
	public GameObject scoreText;
	private GameObject tempText;
	private int textCounter = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		bool empty = true;
		for (int i = 0; i < children.Length; i++) {
			if (children [i] != null) {
				empty = false;
				break;
			}
		}

		if (empty && (tempText == null)) {
			Player.finishTime = (Time.time - Player.timeLost);
			Player.bossDefeated = true;
			tempText = spawnTextAtLocation(new Vector3(0, 0, -1f));
			Player.addScorePoints(10000);
			SpawnOutPixelsBoss.dying = true;
			GameObject.FindObjectOfType<ScoreRecap>().run = true;
		}

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
	}

	private GameObject spawnTextAtLocation(Vector3 location)
	{
		var popUpText = Instantiate(scoreText, location, Quaternion.identity) as GameObject;
		Debug.Log (popUpText);

		popUpText.transform.parent = gameObject.transform;
		popUpText.transform.localPosition = new Vector3(location.x, location.y + 1.5f, -3f);

		popUpText.GetComponent<TextMesh>().text = "10000";

		popUpText.SetActive(true);

		return popUpText;
	}
}
