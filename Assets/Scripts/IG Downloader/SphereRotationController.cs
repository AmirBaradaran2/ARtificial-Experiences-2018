using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereRotationController : MonoBehaviour {

	public float xRotation;
	public float yRotation;
	public float zRotation;

	void Update()
	{
		transform.Rotate (new Vector3(xRotation* Time.deltaTime, yRotation * Time.deltaTime, zRotation * Time.deltaTime));
	}
}
