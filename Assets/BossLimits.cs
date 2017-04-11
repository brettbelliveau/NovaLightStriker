using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLimits : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Physics.IgnoreLayerCollision (9, 17, true);
	}
	
	// Update is called once per frame
	void Update () {

	}
}
