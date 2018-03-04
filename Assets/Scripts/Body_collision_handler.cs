using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body_collision_handler : MonoBehaviour {

    private bool collision_detected = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        collision_detected = true;
    }

    private void OnTriggerExit(Collider other)
    {
        collision_detected = false;
    }

    public bool collision {
        get{
            return collision_detected;
        }
    }
}
