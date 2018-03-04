using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothActions : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<Cloth>().enabled = true;
		gameObject.SetActive(true);
	}

	void OnCollisionEnter (Collision other) {

		Debug.Log("cloth triggering_col with " + other.gameObject.name);
	}

	void OnTriggerEnter (Collider other) {

		Debug.Log("cloth triggering_trig with " + other.gameObject.name);
	}
	// Update is called once per frame
	void Update () {
		
	}
}
