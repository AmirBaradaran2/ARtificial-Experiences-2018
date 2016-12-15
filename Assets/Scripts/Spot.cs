using UnityEngine;
using System.Collections;

public class Spot {
	private string name;
	private string city;
	private string museum;
	private string videoFile;
	private float[] coordinates;

	public Spot(string name, string city, string museum, string videoFile, float[] coordinates) {
		this.name = name;
		this.city = city;
		this.museum = museum;
		this.coordinates = coordinates;
		this.videoFile = videoFile;
	}

	public string GetName() {
		return this.name;
	}

	public string GetCity() {
		return this.city;
	}

	public string GetMuseum() {
		return this.museum;
	}

	public float[] GetCoordinates() {
		return this.coordinates;
	}

	public string GetVideoFile() {
		return this.videoFile;
	}
}
