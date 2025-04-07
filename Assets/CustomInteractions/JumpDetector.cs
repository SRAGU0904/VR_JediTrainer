using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

// https://www.youtube.com/watch?v=GRSOrkmasMM

[RequireComponent(typeof(InputData))]
public class JumpDetector : MonoBehaviour
{

    private InputData _inputData;

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

        _inputData = GetComponent<InputData>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("isgrounded" + isGrounded());
        Debug.Log("iscrouching" + isCrouching());
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
        _inputData._HMD.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 pos);
        Ray ray = new Ray(pos, Vector3.down);
        RaycastHit hit;
        return Physics.Raycast(ray, out hit, crouchThreshold, groundLayer);
    }

}
