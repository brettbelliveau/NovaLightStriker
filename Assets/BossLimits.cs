using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BossLimits : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Physics2D.IgnoreLayerCollision (9, 17, true);
	}
	
	// Update is called once per frame
	void Update () {
		Physics2D.IgnoreLayerCollision (9, 17, true);
	}
}
