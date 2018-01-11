using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameObjWithIGTextureController : MonoBehaviour {
	// reference to the script that contains the Instagram images
	public DownloadIGMedia igDownloadScript;

	void Start () {
		// if no IG download script was set in editor, defaults to the one attached to camera
		if (!igDownloadScript) {
			print ("ig script not attached, attaching");
			igDownloadScript = GameObject.FindGameObjectWithTag ("VRCamera").GetComponent<DownloadIGMedia> ();
		} else {
			print ("ig script already attached");
		}
		StartCoroutine (WaitAndSetTexture ());
	}
		
	// Waits until the IG downloader script has downloaded the IG images, then sets this gameobject's texture to a random texture from the downloaded IG images
	IEnumerator WaitAndSetTexture() {
		print ("Waiting until textures are ready...");
		yield return new WaitUntil (() => igDownloadScript.texturesReady == true);
		print ("Wait has returned, setting my texture to random image from downloaded textures");
		int randNum = Random.Range(0, igDownloadScript.texturesList.Count - 1);
		GetComponent<Renderer> ().material.mainTexture = igDownloadScript.texturesList [randNum];
	}
}
