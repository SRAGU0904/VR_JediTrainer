using System;
using System.Collections.Generic;
using UnityEngine;

// https://www.youtube.com/watch?v=GRSOrkmasMM

public class JumpDetector : MonoBehaviour
{    

    public Transform head;

    public float crouchThreshold = 0.25f;
    public float minJumpCharge = 0.75f;
    public float maxJumpCharge = 3f;

    private float jumpCharge = 0f;

    private LayerMask groundLayer;
    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        groundLayer = LayerMask.GetMask("Ground");
    }

    // Update is called once per frame
    void Update()
    {
        if (isGrounded() && isCrouching()) {
            Debug.Log("charging");
            jumpCharge += Time.deltaTime;
        } else {
            if (jumpCharge > minJumpCharge) {
                jumpCharge = Math.Min(jumpCharge, maxJumpCharge);
                rb.AddForce(Vector3.up * jumpCharge, ForceMode.Impulse);
                Debug.Log("JUMP");
            }
            jumpCharge = 0f;
        }
    }

    // from https://www.youtube.com/watch?v=Xf2eDfLxcB8
    bool isGrounded() {
        return Physics.CheckSphere(transform.position, 0.2f, groundLayer);
    }

    bool isCrouching() {
        Ray ray = new Ray(head.position, Vector3.down);
        RaycastHit hit;
        return Physics.Raycast(ray, out hit, crouchThreshold, groundLayer);
    }

}
