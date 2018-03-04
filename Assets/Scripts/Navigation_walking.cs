using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation_walking : MonoBehaviour {

    public float speed = 0.001f;
    public float sensitivity = 2;

    public GameObject body;

    public Rigidbody rb;

    private CharacterController player;

    private SteamVR_TrackedObject tracked_obj;
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)tracked_obj.index); } }

    private Valve.VR.EVRButtonId trigger_btn = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;

    private Valve.VR.EVRButtonId grip_btn = Valve.VR.EVRButtonId.k_EButton_Grip;

    // Use this for initialization
    void Start () {
        tracked_obj = GetComponent<SteamVR_TrackedObject>();
        if (body != null)
            player = body.GetComponent<CharacterController>();
    }
	
	// Update is called once per frame
	void Update () {
        if (body != null)
            player = body.GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.Log("Controller is null");
            return;
        }

        // if (controller.GetPress(trigger_btn) || controller.GetPress(grip_btn))
        if (controller.GetPress(grip_btn))
        {
            Vector3 rotation_xyz_angles = transform.rotation.eulerAngles;
            Quaternion rotation_yz = Quaternion.Euler(0, rotation_xyz_angles.y, rotation_xyz_angles.z);
            Vector3 move_direction = new Vector3(0, rotation_xyz_angles.y, rotation_xyz_angles.z);
            //move_direction = transform.TransformDirection(move_direction);
            //player.Move(move_direction * speed * Time.deltaTime);
            rb.AddForce(rotation_yz * Vector3.forward * 1, ForceMode.Impulse);
            rb.maxAngularVelocity = 2f;
        }

        if (controller.GetPressUp(trigger_btn))
        { 

        }
        
    }
}
