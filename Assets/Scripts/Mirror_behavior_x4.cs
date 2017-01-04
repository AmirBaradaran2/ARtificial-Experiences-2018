using UnityEngine;
using System.Collections;

public class Mirror_behavior_x4 : MonoBehaviour {

    public int resolution = 2048;

    private RenderTexture texture;
    public Camera camera;
    public MeshRenderer renderer_1;
    public MeshRenderer renderer_2;
    public MeshRenderer renderer_3;
    public MeshRenderer renderer_4;
    public int offset = 1;


    // Use this for initialization
    void Start () {
        texture = new RenderTexture(resolution, resolution, 16);
        texture.name = "__Mirror" + GetInstanceID();
        Debug.Log("Render Texture");
        camera.targetTexture = texture;

        RenderTexture.active = texture;
        int width = texture.width;
        int height = texture.height;

        Texture2D original_texture = new Texture2D(width, height);
        original_texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        original_texture.Apply();

        renderer_1.materials[0].mainTexture 
            = clip_expected_texture(original_texture, 0, 0, width/2, height/2);
        renderer_2.materials[0].mainTexture 
            = clip_expected_texture(original_texture, width/2, 0, width/2, height/2);
        renderer_3.materials[0].mainTexture 
            = clip_expected_texture(original_texture, 0, height/2, width/2, height/2);
        renderer_4.materials[0].mainTexture 
            = clip_expected_texture(original_texture, width/2, height/2, width/2, height/2);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    Texture2D clip_expected_texture(Texture2D texture, int x, int y, int width, int height) {
        Color[] pixels = texture.GetPixels(x,y,width,height);
        Texture2D clipped_texture = new Texture2D(width, height);
        clipped_texture.SetPixels(pixels);
        clipped_texture.Apply();
        return clipped_texture;
    }
}
