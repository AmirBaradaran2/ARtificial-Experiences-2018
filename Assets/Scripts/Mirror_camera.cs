using UnityEngine;
using System.Collections;

public class Mirror_camera : MonoBehaviour {

    public GameObject followed_obj;
    public Camera cam;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        cam.transform.LookAt(followed_obj.transform.position);
	}
}
