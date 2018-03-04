using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCameraController : MonoBehaviour {

    [SerializeField] private Transform target;
    public Vector3 offsetPos;
    public Vector3 targetOffset;
    public float moveSpeed = 5;
    public float turnSpeed = 10;
    public float smoothSpeed = 0.5f;

    private Quaternion targetRotation;
    private Vector3 targetPos;
    private Vector3 defaultPos;
    private Quaternion defaultRotation;
    private bool following = false;

    private void Start() {
        defaultPos = transform.position;
        defaultRotation = transform.rotation;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F)) {
            following = !following;
            if (!following) {
                transform.position = defaultPos;
                transform.rotation = defaultRotation;
            }
        }
    }

    private void FixedUpdate() {
        if (following) {
            movewithTarget();
            lookatTarget();
        }

        
    }

    private void movewithTarget() {
        targetPos = target.position + offsetPos;
        transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }

    private void lookatTarget() {
        targetRotation = Quaternion.LookRotation((target.position + targetOffset) - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }
    
}
