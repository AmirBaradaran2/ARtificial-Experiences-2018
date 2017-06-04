using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brush_tail : MonoBehaviour {

    private TrailRenderer rend;

	// Use this for initialization
	void Start () {
        rend = GetComponent<TrailRenderer>();
        rend.startWidth = 0.08f;
        rend.endWidth = 0.0f;
        rend.startColor = Color.black;
        rend.endColor = Color.white;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
