using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ControllerFunctions : MonoBehaviour
{

    private SteamVR_TrackedObject trackedObj;   

    HashSet<InteractableItem> objectsHoveringOver = new HashSet<InteractableItem>();

    private ControllerEvents controller;
    private GameObject centerPoint;
    private Transform controllerTransform;
    private InteractableItem closestItem;
    private InteractableItem collidedItem;
    public InteractableItem interactingItem;
    private Vector3 originalPosition;
    private Vector3 originalAnchorPosition;
    private Vector3 newPosition;
    private Vector3 originalRotationVector;
    private Vector3 oldRotationVector;
    private Vector3 newRotationVector;
    private Vector3 originalCenterPosition;
    private Vector3 newCenterPosition;
    //private Rigidbody secondWandRigidBody;

    private Matrix4x4 controllerMatrix;
    private Matrix4x4 objectOffsetMatrix;
    
    private float originalDistance;
    private float oldDistance;

    [HideInInspector]
    public bool controllerActive = false;
    [HideInInspector]
    public bool originalParamatersSet = false;
    [HideInInspector]
    public bool haltRotation = false;
    [HideInInspector]
    public bool triggerPulled = false;
    [HideInInspector]
    public bool isGripping = false;
    [HideInInspector]
    public bool collisionTriggered = false;
    [HideInInspector]
    public bool isScaling = false;
    [HideInInspector]
    public bool isRotating = false;
    [HideInInspector]
    public bool isAttached = false;
    [HideInInspector]
    public bool isOffhand = false;
    [HideInInspector]
    public bool delayedExit = false;
    [HideInInspector]
    public bool collisionFromKey = false;
    [HideInInspector]
    private bool offsetLocked = false;
    //public bool enableSnapping = false;

    public void Awake()
    {
        controller = GetComponent<ControllerEvents>();
        controllerTransform = GetComponent<Transform>();
        controller.onWandInitialized += this.wandInitialized;
        controller.onWandLost += this.wandLost;
    }

    // Use this for initialization
    void Start()
    {        

    }
   
    public void wandInitialized(object sender)
    {
        controller.onTriggerPressed += this.triggerPressed;
        controller.onTriggerReleased += this.triggerReleased;
        controller.onGripPressed += this.gripPressed;
        controller.onGripReleased += this.gripReleased;
    }
    public void wandLost(object sender)
    {
        controller.onTriggerPressed -= this.triggerPressed;
        controller.onTriggerReleased -= this.triggerReleased;
        controller.onGripPressed -= this.gripPressed;
        controller.onGripReleased -= this.gripReleased;
    }
    public void triggerPressed(object sender)
    {
        triggerPulled = true;
    }
    public void triggerReleased(object sender)
    {
        
        //Dropping from main-hand
        if (offsetLocked)
        {
            Debug.Log("Setting rb");
            offsetLocked = false;
            interactingItem.gameObject.GetComponent<Rigidbody>().isKinematic = interactingItem.origIsKinematicSetting;
        }

        if (isAttached)
        {
            dropObject();
        }

        if (isRotating && (collidedItem.GetInteractingController() != this))
        {
            collidedItem.activeBimanualInteraction = false;
            collidedItem.fromBiManualScaling = true;
            calculateOffset();
        }

        if (isOffhand)
        {
            isScaling = false;
            isRotating = false;
            isOffhand = false;
            interactingItem = null;
        }

        triggerPulled = false;
       
        if (originalParamatersSet)
        {
            originalParamatersSet = false;
        }
    }

    public void gripPressed(object sender)
    {
        isGripping = true;
    }
    public void gripReleased(object sender)
    {
        isGripping = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (controller == null)
        {
            Debug.Log("Controller not initializad");
            return;
        }

        //if controller.triggerPulled, then attach        
        if (this.triggerPulled)
        {
            Debug.Log("#Interactable items = " + objectsHoveringOver.Count);
            float minDistance = float.MaxValue;
            float distance;
            //Determine which object to pickup
            foreach (InteractableItem item in objectsHoveringOver)
            {
                distance = (item.transform.position - transform.position).sqrMagnitude;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestItem = item;
                }
                interactingItem = closestItem;
            }

            if (interactingItem)
            {
                if (!interactingItem.IsInteracting())
                {
                    interactingItem.BeginInteraction(this);
                    isAttached = true;

                    if (isOffhand)
                    {
                        handOff();
                    }
                }
            }

            //Prepare for single handed grab
            if (interactingItem && this == interactingItem.GetInteractingController() && !offsetLocked)
            {
                calculateOffset();
                offsetLocked = true;
                interactingItem.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                Debug.Log("preparation finished");
            }
            //Grab object
            if (interactingItem && this == interactingItem.GetInteractingController() && offsetLocked && !interactingItem.activeBimanualInteraction)
            {
                if (collidedItem.fromBiManualScaling)
                {
                    calculateOffset();
                    collidedItem.fromBiManualScaling = false;
                }
                Matrix4x4 controllerMatrix = transform.localToWorldMatrix;
                Matrix4x4 newObjectMatrix = controllerMatrix * objectOffsetMatrix;

                collidedItem.transform.position = extractPosition(newObjectMatrix);
                collidedItem.transform.rotation = extractRotation(newObjectMatrix);
            }
        }

        //Force exit when TriggerOnExit isn't called
        if (delayedExit)
        {
            if (isOffhand && !isScaling && !isRotating)
            {
                collisionTriggered = false;
                objectsHoveringOver.Remove(collidedItem);
                interactingItem = null;
                delayedExit = false;
            }

            if (isAttached)
            {
                delayedExit = false;
                collisionTriggered = false;
            }

            if (!isAttached && !isOffhand)
            {
                collisionTriggered = false;
                objectsHoveringOver.Remove(collidedItem);
                interactingItem = null;
                delayedExit = false;
            }
        }

        //All requirements met for scaling & rotating to begin, set initial scaling parameters
        if (triggerPulled && collisionTriggered && collidedItem != null && !originalParamatersSet &&
            collidedItem.GetInteractingController() && (collidedItem.GetInteractingController() != this))
        {
            setOriginalParameters();
            isScaling = true;
            isRotating = true;
            originalParamatersSet = true;
            isOffhand = true;
            collidedItem.activeBimanualInteraction = true;
        }
        //Begin scaling & rotation
        if (triggerPulled && isScaling && isRotating && collisionTriggered && (collidedItem != null) &&
            collidedItem.GetInteractingController())
        {
            updatePosition();
            startScaling(collidedItem.GetInteractingController(), collidedItem);
            startRotating(collidedItem.GetInteractingController(), collidedItem);
        }
        //cleanup gameobjects used for rotating
        if(centerPoint && !isRotating)
        {
            Destroy(centerPoint);
        }
    }
    
    protected void OnTriggerEnter(Collider collider)
    {
        Debug.Log("On trigger enter");
        if (!isAttached)
        {
            collidedItem = collider.GetComponent<InteractableItem>();
            collisionTriggered = true;
            if (collidedItem)
            {
                objectsHoveringOver.Add(collidedItem);
            }
        }      
    }

    //OnTriggerExit fires when the controller Collider impacts another collider and you pull the trigger. Had to 
    //work around it with delayedExit
    protected void OnTriggerExit(Collider collider)
    {
        Debug.Log("On trigger exit");
        //if (!collisionFromKey)                                  DO NOT DELETE
        //{
        InteractableItem collidedItem = collider.GetComponent<InteractableItem>();
        //Dont Trigger the exit unless the scaling Trigger is NOT pressed
        if (isOffhand)
        {
            if (!isScaling && !isRotating)
            {
                collisionTriggered = false;
                objectsHoveringOver.Remove(collidedItem);
            }
            else
            {
                delayedExit = true;
            }
        }

        if (!isAttached && !isOffhand)
        {
            collisionTriggered = false;
            objectsHoveringOver.Remove(collidedItem);
        }
        if (isAttached)
        {
            delayedExit = true;
        }
        //}                                                        DO NOT DELETE
    }

    public void startScaling(ControllerFunctions interactingWand, InteractableItem collidedItem)
    {
        Transform interactingPoint = interactingWand.transform;

        float currentDistance = (controllerTransform.position - interactingWand.transform.position).magnitude;
        float scalingFactor = (currentDistance / oldDistance);

        Vector3 itemScale = collidedItem.transform.localScale;
        Vector3 controllerToCube = collidedItem.transform.position - controllerTransform.position;

        if (scalingFactor - 1 > .1F || scalingFactor - 1 > -.1F)
        {
            collidedItem.transform.localScale = new Vector3(itemScale.x * scalingFactor, itemScale.y * scalingFactor, itemScale.z * scalingFactor);
        }
        oldDistance = currentDistance;
    }

    private void setOriginalParameters()
    {
        if (!centerPoint)
        {
            centerPoint = new GameObject();
        }

        originalPosition = controllerTransform.position;
        originalAnchorPosition = collidedItem.GetInteractingController().transform.position;
        originalCenterPosition = (originalPosition + originalAnchorPosition) / 2;
        originalDistance = (controllerTransform.position - collidedItem.GetInteractingController().transform.position).magnitude;
        originalRotationVector = originalPosition - originalAnchorPosition;
        oldDistance = originalDistance;

        centerPoint.transform.position = originalCenterPosition;
        Matrix4x4 centerPointMatrix = centerPoint.transform.localToWorldMatrix;

        Matrix4x4 invCenterPtMatrix = centerPointMatrix.inverse;
        Matrix4x4 objectMatrix = collidedItem.transform.localToWorldMatrix;
        objectOffsetMatrix = invCenterPtMatrix * objectMatrix;

        oldRotationVector = originalRotationVector;
        newRotationVector = originalRotationVector;
        newCenterPosition = originalCenterPosition;
    }

    private void updatePosition()
    {
        newPosition = controllerTransform.position;
        newCenterPosition = (collidedItem.GetInteractingController().transform.position + newPosition) / 2;       
    }

    
    public void startRotating(ControllerFunctions interactingWand, InteractableItem collidedItem)
    {
       
        //Create GameObject for center point, use Matrix math to find offset of object from centerpoint
        centerPoint.transform.position = newCenterPosition;
        Matrix4x4 newCenterTransformMatrix = centerPoint.transform.localToWorldMatrix;
        Matrix4x4 newObjectMatrix = newCenterTransformMatrix * objectOffsetMatrix;
        collidedItem.transform.position = extractPosition(newObjectMatrix);
        
        //Calculate Rotation
        newRotationVector = this.transform.position - collidedItem.GetInteractingController().transform.position;
        Vector3 axisRotationVector = Vector3.Cross(oldRotationVector, newRotationVector);
        float angle = Vector3.Angle(oldRotationVector, newRotationVector);
        Quaternion differentialRotation = Quaternion.AngleAxis(angle, axisRotationVector);
        collidedItem.transform.rotation = differentialRotation * collidedItem.transform.rotation;
        oldRotationVector = newRotationVector;    
    }

    public void dropObject()
    {
        if (isAttached)
        {
            isAttached = false;
            collidedItem.EndInteraction(this);
            interactingItem = null;
            objectsHoveringOver.Remove(collidedItem);

            //If dropping a snapKey, turn off collision rerouting from Key object
            if (collisionFromKey)
            {
                collisionFromKey = false;
            }
        }
    }

    public Quaternion extractRotation(Matrix4x4 matrix)
    {
        Vector3 forward;
        forward.x = matrix.m02;
        forward.y = matrix.m12;
        forward.z = matrix.m22;
        Vector3 upwards;
        upwards.x = matrix.m01;
        upwards.y = matrix.m11;
        upwards.z = matrix.m21;
        return Quaternion.LookRotation(forward, upwards);
    }

    public Vector3 extractPosition(Matrix4x4 matrix)
    {
        float x = matrix.m03;
        float y = matrix.m13;
        float z = matrix.m23;
        return new Vector3(x, y, z);
    }

    public void calculateOffset()
    {
        Matrix4x4 objectMatrix = collidedItem.transform.localToWorldMatrix;
        Matrix4x4 invObjectMatrix = objectMatrix.inverse;
        if (collidedItem.GetInteractingController())
        {
            controllerMatrix = collidedItem.GetInteractingController().transform.localToWorldMatrix;
        }
        
        Matrix4x4 invControllerMatrix = controllerMatrix.inverse;    
        objectOffsetMatrix = invControllerMatrix * objectMatrix;
    }

    public void handOff()
    {
        isOffhand = false;
        isRotating = false;
        isScaling = false;
        offsetLocked = false;
        collidedItem.activeBimanualInteraction = false;
    }

    //Rerouted collision from InteractableItem script(line ~70) 
    public void TriggerOnEnter(Collider collider)
    {
        collisionFromKey = true;
        OnTriggerEnter(collider);
    }
    //Rerouted collision from InteractableItem script(line ~70)
    public void TriggerOnExit(Collider collider)
    {
        //Debug.Log("CF 470: TriggerOnExit at this position: " + collider.transform.position);
        //collisionFromKey = false;
        //OnTriggerExit(collider);
    }
}
