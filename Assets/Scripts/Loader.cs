using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour {

	public GameObject soundManager;

	void Awake () {
		if (soundManager && SoundManager.instance == null)
			Instantiate(soundManager);
	}
}