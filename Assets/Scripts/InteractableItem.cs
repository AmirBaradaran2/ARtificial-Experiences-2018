using UnityEngine;
using System.Collections;

public class InteractableItem : MonoBehaviour
{

    public new Rigidbody rigidbody;
    private ControllerFunctions attachedWand;
    private ControllerFunctions scalingWand;

    public Material[] materials;
    private Renderer renderer;
    
    public bool isScalable = true;

    [HideInInspector]
    public bool currentlyInteracting;
    [HideInInspector]
    public bool activeBimanualInteraction = false;
    [HideInInspector]
    public bool fromBiManualScaling = false;
    [HideInInspector]
    public bool isKey = false;
    [HideInInspector]
    public bool origGravitySetting;
    [HideInInspector]
    public bool origIsKinematicSetting;


    // Use this for initialization
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        origGravitySetting = rigidbody.useGravity;
        origIsKinematicSetting = rigidbody.isKinematic;

        if(materials.Length > 0) {
            renderer = GetComponent<Renderer>();
            renderer.sharedMaterial = materials[0];
        }
            
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BeginInteraction(ControllerFunctions wand)
    {
        attachedWand = wand;
        currentlyInteracting = true;

        if (materials.Length > 0) {
            renderer.sharedMaterial = materials[1];
        }
    }

    public void EndInteraction(ControllerFunctions wand)
    {
        if (wand == attachedWand)
        {
            attachedWand = null;
            currentlyInteracting = false;
            rigidbody.useGravity = origGravitySetting;
            rigidbody.isKinematic = origIsKinematicSetting;

            if (materials.Length > 0) {
                renderer.sharedMaterial = materials[0];
            }
        }
    }

    public bool IsInteracting()
    {
        return currentlyInteracting;
    }

    public ControllerFunctions GetInteractingController()
    {
        return attachedWand;
    }

    //When object acts as Key, the controllers can't pick up the collision, 
    //so object must reroute collision message
    public void OnTriggerEnter(Collider collider)
    {
        if (isKey && collider.gameObject.tag == "GameController")
        {
            collider.GetComponent<ControllerFunctions>().TriggerOnEnter(gameObject.GetComponent<Collider>());
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (isKey && collider.gameObject.tag == "GameController")
        {
            collider.GetComponent<ControllerFunctions>().TriggerOnExit(gameObject.GetComponent<Collider>());
        }

    }
}