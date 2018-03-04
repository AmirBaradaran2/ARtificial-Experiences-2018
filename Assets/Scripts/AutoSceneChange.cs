using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AutoSceneChange : MonoBehaviour {

	public int seconds;
	public string sceneName;

	IEnumerator Start () {
		yield return new WaitForSeconds (seconds);
		SceneManager.LoadScene (sceneName);
	}

}
