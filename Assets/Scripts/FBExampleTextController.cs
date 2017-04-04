using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;

public class FBExampleTextController : MonoBehaviour {

	Text textDisplayed;
	string FBAccessTokenKey = "Facebook Access Token";

	void Awake () {
		if (!FB.IsInitialized) {
			// Initialize the Facebook SDK
			FB.Init (InitCallback);
		} else {
			// Already initialized, signal an app activation App Event
			FB.ActivateApp ();
			showFBText();
		}
	}

	private void InitCallback ()
	{
		if (FB.IsInitialized) {
			// Signal an app activation App Event
			FB.ActivateApp();
			showFBText();
		} else {
			Debug.Log("Failed to Initialize the Facebook SDK");
		}
	}
		
	void showFBText () {
		textDisplayed = gameObject.GetComponent<Text>(); 

		// retrieving FB access token from player prefs
		if (PlayerPrefs.HasKey (FBAccessTokenKey)) {
			Debug.Log ("FB Access Token exists: " + PlayerPrefs.GetString(FBAccessTokenKey));

			// data we pass to the API request
			var formData = new Dictionary<string,string> () { 
				{ "fields", "name,email"},
				{ "access_token", PlayerPrefs.GetString (FBAccessTokenKey) } };

			// request info from the API
			FB.API ("/me", HttpMethod.GET, APICallback, formData);

		} else {
			Debug.Log ("No FB Access Token found");
			textDisplayed.text = "No FB Access Token found, please go back and log in with Facebook";
		}
	}

	// called when FB API call returns
	private void APICallback(IGraphResult result) {
		if (result.Error == null) {
			Debug.Log ("FB API response: " + result.RawResult);
			textDisplayed.text = result.RawResult;
		} else {
			Debug.Log ("received error from api");
		}
	}
}
