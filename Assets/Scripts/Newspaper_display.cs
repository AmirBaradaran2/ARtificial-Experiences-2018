using UnityEngine;
using System.Collections;

public class Newspaper_display : MonoBehaviour {

    public string url = "http://www.nytimes.com";

    IEnumerator Start() {
        WWW www = new WWW(url);
        yield return www;
        Renderer render = GetComponent<Renderer>();
        render.material.mainTexture = www.texture;
    }
}
