using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformBG : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<SpriteRenderer> ().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		GameObject player = GameObject.Find ("Player");
		transform.position = new Vector3 (player.transform.position.x, player.transform.position.y);
	}
}
