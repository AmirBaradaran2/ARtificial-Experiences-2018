using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasLookat : MonoBehaviour {

	[SerializeField] private GameObject character;
    
    [SerializeField] private bool x_fixed = false;
    [SerializeField] private bool y_fixed = false;
    [SerializeField] private bool z_fixed = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Quaternion look_rotation = Quaternion.LookRotation (transform.position - character.transform.position);
		transform.rotation = Quaternion.Slerp (transform.rotation, look_rotation, Time.deltaTime*5f);
        if (x_fixed)
            transform.rotation = Quaternion.Euler(0, transform.rotation.y, transform.rotation.z);
        if (y_fixed)
            transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);
        if (z_fixed)
            transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 0);
    }
}
