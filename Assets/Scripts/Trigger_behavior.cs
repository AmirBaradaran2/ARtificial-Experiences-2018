using UnityEngine;
using System.Collections;

public class Trigger_behavior : MonoBehaviour {

    public Material[] materials;
    private Renderer renderer;

	// Use this for initialization
	void Start () {
        renderer = GetComponent<Renderer>();
        renderer.sharedMaterial = materials[0];
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter() {
        Debug.Log("Find collision");
        renderer.sharedMaterial = materials[1];
    }

    void OnCollisionExit() {
        Debug.Log("End collision");
        renderer.sharedMaterial = materials[0];
    }
}
