using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainUI : MonoBehaviour {

	public void changeScene(string sceneName) {
		SceneManager.LoadScene (sceneName);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
