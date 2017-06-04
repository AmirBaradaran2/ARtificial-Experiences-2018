using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wand_behavior : MonoBehaviour {

    private SteamVR_TrackedObject tracked_obj;
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)tracked_obj.index); } }

    private Valve.VR.EVRButtonId grip_btn = Valve.VR.EVRButtonId.k_EButton_Grip;
    private bool grip_btn_down = false;
    private bool grip_btn_pressed = false;

    private Valve.VR.EVRButtonId trigger_btn = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private bool trigger_btn_down = false;
    private bool trigger_btn_pressed = false;

    private GameObject wand;
    private GameObject pointer;
    private GameObject brush;
    private GameObject Text;
    private static int WAND = 1;
    private static int POINTER = 2;
    private static int BRUSH = 3;
    private static int wand_mode = 0;

    private GameObject current_cursor;
    private float laser_default_length;
    private const float offset = 0.1f;

    private static bool painting = false;
    private LineRenderer line;
    private List<Vector3> points;

    private ControllerFunctions drag_script;

    // Use this for initialization
    void Start () {
        tracked_obj = GetComponent<SteamVR_TrackedObject>();
        drag_script = GetComponent<ControllerFunctions>();
        wand = GameObject.Find("Wand");
        pointer = GameObject.Find("Pointer");
        current_cursor = GameObject.Find("Cursor");
        brush = GameObject.Find("Brush");
        Text = GameObject.Find("Text");
        //laser_default_length = pointer.GetComponent<Renderer>().bounds.size.z; //print("default length = " + laser_default_length);
        if (wand != null && pointer != null && brush != null && Text != null) { // hide the wand
            wand.SetActive(false);
            pointer.SetActive(false);
            brush.SetActive(false);
            Text.SetActive(false);
        }
        else
            Debug.Log("No wand object assigned!");

        init_brush();
    }

    void init_brush() {
        line = brush.AddComponent<LineRenderer>();
        line.material = Resources.Load("Pink") as Material;
        line.startWidth = 0.0f;
        line.endWidth = 0.01f;
        line.numPositions = 0;
        //line.startColor = Color.red;
        //line.endColor = Color.yellow;
        points = new List<Vector3>();
    }
	
	// Update is called once per frame
	void Update () {
        if (controller == null) {
            Debug.Log("Controller is null"); return;
        }

        if (controller.GetPressDown(grip_btn)) {
            // grip button down -> change wand
            grip_btn_down = true;
            if (wand != null)
                change_wand();
            else
                Debug.Log("No wand object assigned!");
        }

        if (controller.GetPressUp(grip_btn)) {
            // grip button up -> do nothing
            grip_btn_down = false;
        }

        if (drag_script.num_interactables > 0 && controller.GetPressDown(trigger_btn)) {
            trigger_btn_pressed = true; //Debug.Log("Pressed");
            if (wand_mode == 3) {
                painting = true; 
                line.numPositions = 0;
                points.RemoveRange(0, points.Count);
                //line.startColor = Color.red;
                //line.endColor = Color.yellow;
            }

            if (wand_mode == 2)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit))
                {
                    if (hit.collider.gameObject.name.Equals("New_york_times"))
                    {
                        List<Region_map> map = hit.collider.gameObject.GetComponent<Ocr>().word_map;
                        if (map != null)
                        {
                            Vector2 hit_coord = hit.textureCoord; //get the hit uv-coordinate on the texture
                            Debug.Log("Hit coord: " + hit_coord);
                            GameObject webpage = hit.collider.gameObject;
                            Texture2D original_texture = webpage.GetComponent<Ocr>().texture;
                            float texture_x = hit_coord.x * original_texture.width;
                            float texture_y = (1 - hit_coord.y) * original_texture.height;
                            Vector2 texture_point = new Vector2(texture_x, texture_y);
                            Debug.Log("Texture coord: " + texture_x + ", " + texture_y);
                            foreach (Region_map m in map)
                            {
                                if (webpage.GetComponent<Ocr>().within_region(texture_point, m.getRect()))
                                {
                                    string word = m.getWord();
                                    Debug.Log("Clicking on " + word);
                                    GameObject text = GameObject.Instantiate(webpage.GetComponent<Ocr>().prefab, hit.point, Quaternion.Euler(new Vector3(0, -90, 0)));
                                    text.GetComponent<TextMesh>().text = word;
                                    text.AddComponent<BoxCollider>();
                                    //render_text(hit.point, word);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        if (controller.GetPressUp(trigger_btn)) {
            trigger_btn_pressed = false;
            if (wand_mode == 3 && painting) {
                painting = false;
            }
        }

        if (painting) {
            Debug.Log("Draw point");
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit)) {
                //draw_brush(hit.point);
                if (!points.Contains(hit.point)) {
                    points.Add(hit.point); Debug.Log("#points = " + points.Count);
                    line.numPositions = points.Count;
                    line.SetPosition(points.Count-1, points[points.Count-1]);
                }
            }
        }
    }

    void change_wand() {
        switch (wand_mode) {
            case 0:
                active_select(); break; 
            case 1:
                active_recognize(); break;
            case 2:
                active_brush();  break;
            case 3:
                deactive_wand(); break;
            default: break;
        }
    }

    public void active_select() {
        Debug.Log("Wand");
        wand.SetActive(true);
        brush.SetActive(false);
        pointer.SetActive(false);
        wand_mode++; 
    }

    public void active_recognize() {
        Debug.Log("Pointer");
        wand.SetActive(false);
        pointer.SetActive(true);
        brush.SetActive(false);
        //Text.SetActive(true);
        wand_mode++; 
    }

    public void active_brush() {
        Debug.Log("Brush");
        wand.SetActive(false);
        pointer.SetActive(false);
        //Text.SetActive(false);
        brush.SetActive(true);
        wand_mode++; 
    }

    public void deactive_wand() {
        brush.SetActive(false);
        pointer.SetActive(false);
        wand.SetActive(false);
        wand_mode = 0;
    }

    void OnTriggerEnter(Collider other_col) {
        //Debug.Log("Trigger enter");
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit)) {
            if (wand_mode == 2 || wand_mode == 3) {
                //draw_cursor(hit.point);
                print("hit distance = " + hit.distance);
                if (hit.distance < laser_default_length) {
                    Vector3 current_scale = pointer.transform.localScale;
                    Vector3 current_pos = pointer.transform.position;
                    float y_p = current_scale.y * hit.distance / laser_default_length - offset;
                    //pointer.transform.localScale = new Vector3(current_scale.x, y_p, current_scale.z);
                    //pointer.transform.Translate(Vector3.back * (current_scale.y-y_p));
                }
            }
            if (wand_mode == 3) {
                //painting = true;
                //draw_brush(hit.point);
            }
        }
    }

    void OnTriggerExit(Collider other_col) {
        //Debug.Log("Trigger exit");
        painting = false;
    }

    /*void draw_cursor(Vector3 col_pos) { //draw cursor at the collision position
        if (current_cursor != null)
            Destroy(current_cursor);
        current_cursor = GameObject.Instantiate(current_cursor, col_pos, 
            Quaternion.Euler(new Vector3(90.0f, 0.0f, 90.0f))) as GameObject;
    }*/

    void draw_brush(Vector3 point) {
        GameObject new_brush;
        new_brush = Instantiate(Resources.Load("Brush_dot"), point + new Vector3(0.1f, 0.0f, 0.0f), Quaternion.Euler(new Vector3(0.0f, 90.0f, 0.0f))) as GameObject;
    }       
}
