using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum Orientation {
	Left,
	Right,
	Up,
	Down,
	None
}

public class AccelerationTip : MonoBehaviour {
	private int tipTimes;
	private Text textComp;
	private CoroutineQueue queue;

	public float sentencePause = 0.2f;
	public float letterPause = 0.01f;
	public AudioClip typeSound1;
	public AudioClip typeSound2;

	private Stack<Orientation> tips;

	private float rotationSinceLastTip;
	private float secondSinceLastTip;
	private Orientation currentTip;
	private Orientation lastTip;

	public static string[] tipString = new string[]{ "look left!", "look right!", "look up at the sky!", "look down under your feet!" };
	public static string[] tipAgainString = new string[]{ "left", "right", "up", "down" };

	private Spot spot;
	private bool finished;

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

	// Use this for initialization
	void Start () {
		Input.gyro.enabled = true;
		finished = false;
		spot = GlobalConstants.locations [GlobalConstants.SPOT_NAME];

		textComp = GetComponent<Text>();
		// use "this" monobehaviour as the coroutine owner
		queue = new CoroutineQueue( this );
		queue.StartLoop();

		System.Random rnd = new System.Random(System.DateTime.Now.Millisecond);
		tipTimes = rnd.Next(1, 4); // 1 2 3

		tips = new Stack<Orientation> ();

		for (int i = 0; i < tipTimes; ++i) {
			tips.Push ((Orientation)rnd.Next (0, 4));
		}
		lastTip = Orientation.None;
		currentTip = tips.Pop ();
		rotationSinceLastTip = 0.0f;
		secondSinceLastTip = 0.0f;
		Debug.Log("Acceleration: current tip " + currentTip);
		queue.EnqueueAction (TypeText (tipString [(int)currentTip]));
		queue.EnqueueWait (sentencePause);
	}

	bool check() {
		if (currentTip == Orientation.Down) {
			return Input.acceleration.z < -0.85f;
		} else if (currentTip == Orientation.Up) {
			return Input.acceleration.z > 0.85f;
		} else if (currentTip == Orientation.Right) {
			while (rotationSinceLastTip > 90.0f) {
				rotationSinceLastTip -= 180.0f;
			}
			return rotationSinceLastTip < -38.0f && rotationSinceLastTip > -52.0f;
		} else if (currentTip == Orientation.Left) {
			while (rotationSinceLastTip < -90.0f) {
				rotationSinceLastTip += 180.0f;
			}
			return rotationSinceLastTip > 38.0f && rotationSinceLastTip < 52.0f;
		}
		return false;
	}

	IEnumerator playVideo() {
		Handheld.PlayFullScreenMovie (spot.GetVideoFile());
		yield return 0;
	}

	// Update is called once per frame
	void Update () {
		if (finished) {
			return;
		}
		secondSinceLastTip += Time.deltaTime;
		rotationSinceLastTip += Input.gyro.rotationRateUnbiased.y;
		if (check()) {
			if (tips.Count > 0) {
				lastTip = currentTip;
				currentTip = tips.Pop ();
				Debug.Log("Acceleration: current tip " + currentTip);
				rotationSinceLastTip = 0.0f;
				secondSinceLastTip = 0.0f;
				if (lastTip == currentTip) {
					queue.EnqueueAction (TypeText ("Sorry, I need you to " + tipString [(int)currentTip] + " again"));
				} else {
					queue.EnqueueAction (TypeText ("Sorry, I need you to " + tipString [(int)currentTip]));
				}
				queue.EnqueueWait (sentencePause);
			} else {
				queue.EnqueueAction (TypeText ("you got it! Watch " + spot.GetName()));
				queue.EnqueueWait (sentencePause);
				queue.EnqueueAction (playVideo ());
				finished = true;
			}
		} else {
			if (secondSinceLastTip > 20.0f) {
				queue.EnqueueAction (TypeText ("I'm still waiting for looking " + tipAgainString [(int)currentTip]));
				queue.EnqueueWait (sentencePause);
				secondSinceLastTip = 0.0f;
				Debug.Log("Acceleration: rotation " + rotationSinceLastTip);
			}
		}
//		textComp.text = "x: " + Input.acceleration.x + "\ny: " + Input.acceleration.y + "\nz: " + Input.acceleration.z;
//		rotationSinceLastTip += Input.gyro.rotationRateUnbiased.y;
//		textComp.text = "" + rotationSinceLastTip;
	}

	void OnDestroy() {
		queue.StopLoop ();
	}
}
