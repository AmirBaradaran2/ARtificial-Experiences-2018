using UnityEngine;
using System.Collections;

public class Mirror_behavior : MonoBehaviour {

    public int resolution = 2048;

    private RenderTexture texture;
    public Camera camera;
    public MeshRenderer renderer;

    // Use this for initialization
    void Start () {
        texture = new RenderTexture(resolution, resolution, 16);
        texture.name = "__Mirror" + GetInstanceID();
        Debug.Log("Render Texture");
        camera.targetTexture = texture;
        renderer.materials[0].mainTexture = texture;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
