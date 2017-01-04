using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class ControllerEvents : MonoBehaviour {

    public delegate void wandEventHandler(object sender);

    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;

    public SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    public SteamVR_TrackedObject trackedObj;

    HashSet<InteractableItem> objectsHoveringOver = new HashSet<InteractableItem>();
    private InteractableItem closestItem;
    private InteractableItem interactingItem;
    private InteractableItem collidedItem;

    public event wandEventHandler onWandInitialized;
    public event wandEventHandler onWandLost;
    public event wandEventHandler onTriggerPressed;
    public event wandEventHandler onTriggerReleased;
    public event wandEventHandler onGripPressed;
    public event wandEventHandler onGripReleased;


    void Awake()
    {
        //Initialize Vive Wand Controllers
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // Use this for initialization
    void Start () { 

        if (trackedObj != null)
        {
            wandInitialized(this);
        }
        else
        {
            Debug.Log("CE: Vive Wand failed to initialize");
        }

    }
	
	// Update is called once per frame
	void Update () {

        if(controller.index >= uint.MaxValue)
        {
            return;
        }

        if (controller == null)
        {
            wandLost(this);
        }        
        if (controller.GetPressDown(triggerButton))
        {
            triggerPressed(this);
        }
        if (controller.GetPressUp(triggerButton))
        {
            triggerReleased(this);
        }
        if (controller.GetPressDown(gripButton))
        {
            onGripPressed(this);
        }
        if (controller.GetPressUp(gripButton))
        {
            onGripReleased(this);
        }
    }
    
    public void wandInitialized(object sender)
    {
        
        if(onWandInitialized != null)
        {
            onWandInitialized(this);
        }        
    }
    public void wandLost(object sender)
    {
        if(onWandLost != null)
        {
            onWandLost(this);
        }
    }

    public void triggerPressed(object sender)
    {
        if(onTriggerPressed != null)
        {
            onTriggerPressed(this);
        }
    }

    public void triggerReleased(object sender)
    {
        if(onTriggerReleased != null)
        {
            onTriggerReleased(this);            
        }
    }
    public void gripPressed(object sender)
    {
        if (onGripPressed != null)
        {
            onGripPressed(this);
        }
    }
    public void gripReleased(object sender)
    {
        if (onGripReleased != null)
        {
            onGripReleased(this);
        }
    }
}