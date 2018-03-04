using UnityEngine;
using System.Collections.Generic;
using Facebook.Unity;

public class LogInButtonTrigger : MonoBehaviour {

	void Awake () {
		if (!FB.IsInitialized) {
			// Initialize the Facebook SDK
			FB.Init (InitCallback);
		} else {
			// Already initialized, signal an app activation App Event
			FB.ActivateApp ();
		}
	}
		
	public void logIn() {
		// Request FB Login with public profile and email permissions
		var perms = new List<string>(){"public_profile", "email"};
		FB.LogInWithReadPermissions(perms, AuthCallback);
	}

	private void InitCallback ()
	{
		if (FB.IsInitialized) {
			FB.ActivateApp();
		} else {
			Debug.Log("Failed to Initialize the Facebook SDK");
		}
	}

	private void AuthCallback (ILoginResult result) {
		if (FB.IsLoggedIn) {
			Debug.Log ("FB log in successful.");
			// AccessToken class will have session details
			var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
			// Print current access token's User ID
			Debug.Log("Access Token user id: " + aToken.UserId);
			Debug.Log ("Access Token: " + aToken.TokenString);

			// Save access token in Unity PlayerPrefs
			PlayerPrefs.SetString("Facebook Access Token", aToken.TokenString);

			Debug.Log ("FB access token saved to player prefs");

			Debug.Log ("Facebook permissions granted:");
			// Print current access token's granted permissions
			foreach (string perm in aToken.Permissions) {
				Debug.Log(perm);
			}
		} else {
			Debug.Log("User cancelled login");
		}
	}
}
