using UnityEngine;
using System.Collections;

public class Navigation_grip : MonoBehaviour {

    private SteamVR_TrackedObject tracked_obj;
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)tracked_obj.index); } }

    public Valve.VR.EVRButtonId grip_btn = Valve.VR.EVRButtonId.k_EButton_Grip;
    public bool grip_btn_down = false;
    public bool grip_btn_up = false;
    public bool grip_btn_pressed = false;

    public Rigidbody rb; 
    public float force = 8;

    // Use this for initialization
    void Start () {
        tracked_obj = GetComponent<SteamVR_TrackedObject>();
    }
	
	// Update is called once per frame
	void Update () {
        if (controller == null) {
            Debug.Log("Controller is null");
            return;
        }

        if (controller.GetPress(grip_btn)) { // grip button pressed -> navigation
            Debug.Log("Grip button pressed. Navigating..");
            rb.AddForce(transform.rotation * Vector3.forward * force);
            rb.maxAngularVelocity = 2f;
        }

        if (controller.GetPressUp(grip_btn)) { // grip button up -> stop navigation
            Debug.Log("Grip button up. Stop navigation.");
        }


    }
}
