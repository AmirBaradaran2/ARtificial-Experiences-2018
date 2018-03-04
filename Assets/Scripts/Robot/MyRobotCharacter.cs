using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyRobotCharacter : MonoBehaviour {

    public float velocity = 5;
    public float turnSpeed = 10;

    private Vector2 input;
    private float angle;

    private Quaternion targetRotation;
    private Transform cam;
    private Animator anim;

    private Vector3 m_move;

    private void Start() {
        cam = Camera.main.transform;
        anim = GetComponent<Animator>();
    }

    private void Update() {
        /*getInput();

        if (Mathf.Abs(input.x) < 1 && Mathf.Abs(input.y) < 1)
            return;

        calculateRotation();
        rotate();
        Move();*/
    }

    private void getInput() {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        anim.SetFloat("BlendX", input.x);
        anim.SetFloat("BlendY", input.y);
    }

    private void updateAnimator() {

    }

    private void calculateRotation(float moveX, float moveZ) {
        angle = Mathf.Atan2(moveX, moveZ);
        angle *= Mathf.Rad2Deg;
        angle += cam.eulerAngles.y;
    }

    private void rotate() {
        targetRotation = Quaternion.Euler(0, angle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }

    private void Move() {
        transform.position += transform.forward * velocity * Time.deltaTime;
    }

    public void Move(Vector3 move, bool sitting, bool lying, bool picking_up) {
        m_move = move.normalized;
        calculateRotation(m_move.x, m_move.z);

        if (m_move.x > 0)   m_move.x = 1;
        if (m_move.x < 0)   m_move.x = -1;
        if (m_move.z > 0)   m_move.z = 1;
        if (m_move.z < 0)   m_move.z = -1;
        anim.SetFloat("BlendX", m_move.x, 0.1f, Time.deltaTime);
        anim.SetFloat("BlendY", m_move.z, 0.1f, Time.deltaTime);
        anim.SetBool("Sitting", sitting); print("Sitting = " + sitting);
        anim.SetBool("Lying", lying); print("Lying = " + lying);
        anim.SetBool("Picking_up", picking_up); print("Picking_up = " + picking_up);

        if (Mathf.Abs(m_move.x) < 1 && Mathf.Abs(m_move.z) < 1)
            return;
        
        rotate();
        Move();
    }
}
