using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;

public class FBExampleTextController : MonoBehaviour {

	Text textDisplayed;
	string FBAccessTokenKey = "Facebook Access Token";

	void Awake () {
		Debug.Log ("fbexamplecontroller awake called");
		Debug.Log ("initializing fb");
		if (!FB.IsInitialized) {
			// Initialize the Facebook SDK
			FB.Init (InitCallback);
		} else {
			// Already initialized, signal an app activation App Event
			Debug.Log("fbexamplecontroller awake: fb already initialized");
			FB.ActivateApp ();
		}

	}

	private void InitCallback ()
	{
		if (FB.IsInitialized) {
			Debug.Log ("initcallback: fb successfully initialized, activating app");

			// Signal an app activation App Event
			FB.ActivateApp();
			// Continue with Facebook SDK
			// ...
			showFBText();

		} else {
			Debug.Log("Failed to Initialize the Facebook SDK");
		}
	}

	// Use this for initialization

	void showFBText () {
		textDisplayed = gameObject.GetComponent<Text>(); 
		Debug.Log ("retrieving FB access token from player prefs");
		Debug.Log(PlayerPrefs.GetString(FBAccessTokenKey));
		if (PlayerPrefs.HasKey (FBAccessTokenKey)) {
			Debug.Log ("FB Access Token exists");

			var formData = new Dictionary<string,string> () { 
				{ "fields", "name,email"},
				{ "access_token", PlayerPrefs.GetString (FBAccessTokenKey) } };

			FB.API ("/me", HttpMethod.GET, APICallback, formData);


		} else {
			Debug.Log ("No FB Access Token found");
			textDisplayed.text = "No FB Access Token found, please go back and log in with Facebook";
		}

	}

	private void APICallback(IGraphResult result) {
		Debug.Log ("api callback");
		if (result.Error == null) {
			Debug.Log (result.ToString());
			Debug.Log (result.RawResult);

			textDisplayed.text = result.RawResult;


		} else {
			Debug.Log ("received error from api");
		}
	}

}
