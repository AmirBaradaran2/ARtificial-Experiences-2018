using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyRobotTurnForwardCharacter : MonoBehaviour {

    [SerializeField] float m_MovingTurnSpeed = 10;
    [SerializeField] float m_StationaryTurnSpeed = 8;
    [SerializeField] float m_AnimSpeedMultiplier = 1f;
    [SerializeField] float m_velocity = 50;

    private Transform m_cam;
    private Animator m_animator;

    private float m_forwardAmount;
    private float m_turnAmount;
    

    // Use this for initialization
    void Start () {
        m_cam = Camera.main.transform;
        m_animator = GetComponent<Animator>();

	}

    public void Move(Vector3 move, bool sitting, bool lying, bool pickingup) {
        Vector3 m_camForward = Vector3.Scale(m_cam.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 m_move = move.normalized;
        m_move = transform.InverseTransformDirection(m_move);

        m_move = Vector3.ProjectOnPlane(m_move, Vector3.up);
        m_turnAmount = Mathf.Atan2(m_move.x, m_move.z);
        m_forwardAmount = m_move.z;

        ApplyExtraTurnRotation();
        UpdateAnimator(m_move, sitting, lying, pickingup);

        if (m_move.magnitude > 0 && m_forwardAmount > 0.5f)
            transform.position += transform.forward * m_velocity * Time.deltaTime;
    }
	
	void FixedUpdate () {
        /*float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 m_camForward = Vector3.Scale(m_cam.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 m_move = v * m_camForward + h * m_cam.right;

        if (m_move.magnitude > 1f) m_move.Normalize();
        m_move = transform.InverseTransformDirection(m_move);

        m_move = Vector3.ProjectOnPlane(m_move, Vector3.up);
        m_turnAmount = Mathf.Atan2(m_move.x, m_move.z);
        m_forwardAmount = m_move.z;

        ApplyExtraTurnRotation();
        UpdateAnimator(m_move, false, false, false);

        if (m_move.magnitude > 0 && m_forwardAmount > 0.5f)
            transform.position +=  transform.forward * m_velocity * Time.deltaTime;*/
    }

    void ApplyExtraTurnRotation() {
        float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_forwardAmount);
        transform.Rotate(0, m_turnAmount * turnSpeed * Time.deltaTime, 0);
    }

    void UpdateAnimator(Vector3 move, bool sitting, bool lying, bool picking_up) {
        if (move.x > 0) move.x = 1;
        if (move.x < 0) move.x = -1;
        if (move.z > 0) move.z = 1;
        if (move.z < 0) move.z = -1;

        m_animator.SetFloat("Forward", m_forwardAmount, 0.1f, Time.deltaTime);
        m_animator.SetFloat("Turn", m_turnAmount, 0.1f, Time.deltaTime);
        m_animator.SetBool("Sitting", sitting); print("Sitting = " + sitting);
        m_animator.SetBool("Lying", lying); print("Lying = " + lying);
        m_animator.SetBool("Picking_up", picking_up); print("Picking_up = " + picking_up);

        if (Mathf.Abs(move.x) < 1 && Mathf.Abs(move.z) < 1)
            return;

        m_animator.speed = m_AnimSpeedMultiplier;
    }
}
