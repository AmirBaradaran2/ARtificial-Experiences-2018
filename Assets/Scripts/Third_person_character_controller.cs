using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Third_person_character_controller : MonoBehaviour {

    public Transform target;

	// Use this for initialization
	void Start () {
        Vector3 relative_pos = target.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relative_pos);
        transform.rotation = rotation;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
