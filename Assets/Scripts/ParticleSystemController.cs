using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemController : MonoBehaviour {

	private ParticleSystem CachedSystem;

	// Use this for initialization
	IEnumerator Start () {
		CachedSystem = GetComponent<ParticleSystem>();

		// waiting a random number of seconds before emitting particles, so the multiple particle systems don't look too synchronized
		int randNum = Random.Range(0, 5);
		print ("Particle system waiting for " + randNum + " seconds before playing");
		yield return new WaitForSeconds (randNum);

		CachedSystem.Play ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
