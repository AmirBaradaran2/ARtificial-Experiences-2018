using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalConstants {
	public static Dictionary<string, Spot> locations = new Dictionary<string, Spot>();

	public static string SPOT_NAME;

	static GlobalConstants() {
		locations.Add ("Frenchising Mona Lisa", 
			new Spot ("Frenchising Mona Lisa", "Paris", "Louvre", "MyMovie3.mp4", new float[2]{48.8606111f, 2.337644f}));
		// 40.8676657f, -73.9669597f -- my home
//		locations.Add ("Frenchising Mona Lisa", 
//			new Spot ("Frenchising Mona Lisa", "Paris", "Louvre", "MyMovie3.mp4", new float[2]{40.8676657f, -73.9669597f}));
		locations.Add ("Take off", 
			new Spot ("Take off", "New York", "MoMA", "weARmoma_takeoff.m4v", new float[2]{40.7614327f,-73.9776216f}));
		// 40.8096293f, -73.9608192f
//		locations.Add ("Take off", 
//			new Spot ("Take off", "New York", "MoMA", "weARmoma_takeoff.m4v", new float[2]{40.8096293f, -73.9608192f}));
	}
}
