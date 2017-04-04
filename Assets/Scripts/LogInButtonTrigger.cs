using UnityEngine;
using System.Collections.Generic;
using Facebook.Unity;

public class LogInButtonTrigger : MonoBehaviour {

	void Awake () {
		Debug.Log ("loginbuttontrigger awake called");
		Debug.Log ("initializing fb");
		if (!FB.IsInitialized) {
			// Initialize the Facebook SDK
			FB.Init (InitCallback);
		} else {
			// Already initialized, signal an app activation App Event
			Debug.Log("awake: fb already initialized");
			FB.ActivateApp ();
		}

	}


	public void logIn() {
		Debug.Log ("requesting login");
		var perms = new List<string>(){"public_profile", "email"};
		FB.LogInWithReadPermissions(perms, AuthCallback);
	}

	private void InitCallback ()
	{
		if (FB.IsInitialized) {
			Debug.Log ("initcallback: fb successfully initialized, activating app");

			// Signal an app activation App Event
			FB.ActivateApp();
			// Continue with Facebook SDK
			// ...
		} else {
			Debug.Log("Failed to Initialize the Facebook SDK");
		}
	}

	private void AuthCallback (ILoginResult result) {
		if (FB.IsLoggedIn) {
			// AccessToken class will have session details
			var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
			// Print current access token's User ID
			Debug.Log("access token user id: " + aToken.UserId);
			Debug.Log ("access token: " + aToken.TokenString);
			// Print current access token's granted permissions
			foreach (string perm in aToken.Permissions) {
				Debug.Log(perm);
			}
		} else {
			Debug.Log("User cancelled login");
		}
	}



}
