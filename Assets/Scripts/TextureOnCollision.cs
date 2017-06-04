using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureOnCollision : MonoBehaviour {

	public Material replaceMat;
	private bool isTriggered = false;
	public Renderer rend;
	private Renderer otherRend;
	public Collider coll;


	// Use this for initialization
	void Start () {
		gameObject.SetActive(true);
		rend = transform.GetComponent<Renderer>();
		coll = GetComponent<SphereCollider> ();
	}
		
	void OnCollisionEnter (Collision other) {

		Debug.Log("triggering_col with " + other.gameObject.name);
		otherRend = other.gameObject.GetComponent<Renderer>();
		replaceMat = otherRend.material;
		isTriggered = true;
	}

	void OnTriggerEnter (Collider other) {

		Debug.Log("triggering_trig with " + other.gameObject.name);
		otherRend = other.gameObject.GetComponent<Renderer>();
		replaceMat = otherRend.material;
		isTriggered = true;
	}

	// Update is called once per frame
	void Update () {
		if (isTriggered) {
			rend.material = replaceMat;
		}
	}
}
	