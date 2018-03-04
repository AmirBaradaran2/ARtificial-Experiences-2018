using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

// Location service not enabled
// Sorry! Timed out
// Unable to determine device location
//
//
public enum LocationState {
	Disabled,
	TimedOut,
	Failed,
	Enabled,
	Stopped
}

public class DisplayManager : MonoBehaviour {

	private LocationState mState;
	private float mLatitude;
	private float mLongitude;
	private float mDistance;
//	private float mHorizontalAccuracy;
//	private double mTimestamp;
	private const float EARTH_RADIUS = 6371.0f;
	private const float KM_2_MILE = 0.621371f;
	private const float KM_2_FOOT = 3280.84f;

	private CoroutineQueue queue;
	private Text textComp;

	public float sentencePause = 0.8f;
	public float letterPause = 0.08f;
	public AudioClip typeSound1;
	public AudioClip typeSound2;

	private Spot spot;
	// [update location, check distance]

	// #1 >= 20 miles
	// you are too far... [change text]
	// get ready for a journey [change text]
	// see you in [city] [change text]
	// [return to MainUI]

	// #2 < 20 miles
	// see you in [location]
	// #3 < 5 miles
	// put on your running shoes or hop on your bike [change text, update location]
	// [update location, check distance in a loop, when half than initialized distance] 
	// you are getting closer now
	// [update location, check distance in a loop, when < 100 feet] 
	// congratulations! you made it!!! [change scene, open camera]

	// #4 < 100 feet
	// Be ready to see [spot]!!!

	IEnumerator Start() {
		textComp = GetComponent<Text>();
		// use "this" monobehaviour as the coroutine owner
		queue = new CoroutineQueue( this );
		queue.StartLoop();

		mState = LocationState.Disabled;
		mLatitude = 0.0f;
		mLongitude = 0.0f;
		mDistance = 0.0f;

		spot = GlobalConstants.locations [GlobalConstants.SPOT_NAME];

		if (Input.location.isEnabledByUser) {
			// Start service before querying location
			Input.location.Start();
			// Wait until service initializes
			int maxWait = 20;
			while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
			{
				yield return new WaitForSeconds(1);
				maxWait--;
			}
			// Service didn't initialize in 20 seconds
			if (maxWait < 1) {
				mState = LocationState.TimedOut;
			} else if (Input.location.status == LocationServiceStatus.Failed) {
				mState = LocationState.Failed;
			} else {
				mState = LocationState.Enabled;
				mLatitude = Input.location.lastData.latitude;
				mLongitude = Input.location.lastData.longitude;
//				mHorizontalAccuracy = Input.location.lastData.horizontalAccuracy;
//				mTimestamp = Input.location.lastData.timestamp;
				mDistance = Haversine ();
			}
		}

		if (mState == LocationState.Enabled) {
			float mileTemp = mDistance * KM_2_MILE;
			float footTemp = mDistance * KM_2_FOOT;
			if (mileTemp > 20.0f) {
				queue.EnqueueAction (TypeText ("you are too far..."));
				queue.EnqueueWait (sentencePause);
				queue.EnqueueAction (TypeText ("get ready for a journey"));
				queue.EnqueueWait (sentencePause);
				queue.EnqueueAction (TypeText ("see you in " + spot.GetCity ()));
				Input.location.Stop ();
				mState = LocationState.Stopped;
			} else if (mileTemp > 5.0f) {
				queue.EnqueueAction (TypeText ("see you in " + spot.GetMuseum ()));
				queue.EnqueueWait (sentencePause);
			} else if (footTemp > 100.0f) {
				queue.EnqueueAction (TypeText ("put on your running shoes or hop on your bike"));
				queue.EnqueueWait (sentencePause);
			} else {
				queue.EnqueueAction (TypeText ("Be ready to see " + spot.GetName()));
				queue.EnqueueWait (sentencePause);
				queue.EnqueueAction (ChangeScene ("GeoCamera")); // this need to change scene
				Input.location.Stop ();
				mState = LocationState.Stopped;
			}
		} else if (mState == LocationState.Disabled) {
			queue.EnqueueAction (TypeText ("Location service not enabled"));
		} else if (mState == LocationState.Failed) {
			queue.EnqueueAction (TypeText ("Unable to determine device location"));
		} else if (mState == LocationState.TimedOut) {
			queue.EnqueueAction (TypeText ("Sorry! Timed out"));
		}
	}

	IEnumerator OnApplicationPause( bool pauseStatus )
	{
		if (pauseStatus) {
			if (mState == LocationState.Enabled) {
				Input.location.Stop ();
				mState = LocationState.Disabled;
			}
		} else {
			if (Input.location.isEnabledByUser && mState != LocationState.Stopped) {
				// Start service before querying location
				Input.location.Start();
				// Wait until service initializes
				int maxWait = 20;
				while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
				{
					yield return new WaitForSeconds(1);
					maxWait--;
				}
				// Service didn't initialize in 20 seconds
				if (maxWait < 1) {
					mState = LocationState.TimedOut;
				} else if (Input.location.status == LocationServiceStatus.Failed) {
					mState = LocationState.Failed;
				} else {
					mState = LocationState.Enabled;
					mLatitude = Input.location.lastData.latitude;
					mLongitude = Input.location.lastData.longitude;
					// mHorizontalAccuracy = Input.location.lastData.horizontalAccuracy;
					// mTimestamp = Input.location.lastData.timestamp;
				}
			}
		}
	}

	IEnumerator ChangeScene(string sceneName) {
		SceneManager.LoadScene(sceneName);
		yield return 0;
	}

	float Haversine() {
		float destLatitude = spot.GetCoordinates()[0];
		float destLongitude = spot.GetCoordinates()[1];

		float deltaLatitude = (destLatitude - mLatitude) * Mathf.Deg2Rad;
		float deltaLongitude = (destLongitude - mLongitude) * Mathf.Deg2Rad;

		float a = Mathf.Pow (Mathf.Sin (deltaLatitude / 2), 2) +
		          Mathf.Cos (mLatitude * Mathf.Deg2Rad) * Mathf.Cos (destLatitude * Mathf.Deg2Rad) *
		          Mathf.Pow (Mathf.Sin (deltaLongitude / 2), 2);

		float c = 2 * Mathf.Atan2 (Mathf.Sqrt (a), Mathf.Sqrt (1 - a));
		return EARTH_RADIUS * c;
	}

	IEnumerator TypeText (string message) {
		textComp.text = "";
		foreach (char letter in message.ToCharArray()) {
			textComp.text += letter;
			if (typeSound1 && typeSound2)
				SoundManager.instance.RandomizeSfx(typeSound1, typeSound2);
			yield return 0;
			yield return new WaitForSeconds (letterPause);
		}
	}

	void OnDestroy() {
		if (mState == LocationState.Enabled) {
			Input.location.Stop ();
		}
		queue.StopLoop ();
	}
	
	// Update is called once per frame
	void Update () {
		if (mState == LocationState.Enabled) {
			mLatitude = Input.location.lastData.latitude;
			mLongitude = Input.location.lastData.longitude;

			float tempDistance = Haversine ();
			if (tempDistance * KM_2_FOOT <= 100.0f) {
				queue.EnqueueAction (TypeText ("congratulations! you made it!!!"));
				queue.EnqueueWait (sentencePause);
				queue.EnqueueAction (ChangeScene ("GeoCamera")); // this need to change scene
				Input.location.Stop ();
				mState = LocationState.Stopped;
			} else if (tempDistance < mDistance * 0.5f) {
				mDistance = tempDistance;
				queue.EnqueueAction (TypeText ("you are getting closer now"));
				queue.EnqueueWait (sentencePause);
			}

		}
	}
}
