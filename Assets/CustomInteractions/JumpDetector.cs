using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;

// https://www.youtube.com/watch?v=GRSOrkmasMM
// https://www.youtube.com/watch?v=Mfim9MlgYWY
// https://www.youtube.com/watch?v=Xf2eDfLxcB8

[RequireComponent(typeof(InputData))]
[RequireComponent(typeof(XROrigin))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class JumpDetector : MonoBehaviour
{

    public TMP_Text debugScreen;

    public Transform toWorld;

    public float crouchThreshold = 0.25f;
    public float minJumpCharge = 0.75f;
    public float maxJumpCharge = 3f;

    private float jumpCharge = 0f;


    private InputData _inputData;
    private XROrigin _xrOrigin;
    private Rigidbody _rb;
    private CapsuleCollider _collider;
    private LayerMask _groundLayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    List<string> debugMessages;
    void Start()
    {
        _inputData = GetComponent<InputData>();
        _xrOrigin = GetComponent<XROrigin>();
        _collider = GetComponent<CapsuleCollider>();
        _rb = GetComponent<Rigidbody>();
        _groundLayer = LayerMask.GetMask("Ground");

        debugMessages = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {
        debugMessages.Clear();
        UpdateColliderPosition();

        // _inputData._HMD.TryGetFeatureValue(CommonUsages.deviceAngularAcceleration, out Vector3 angAcc);
        // _inputData._HMD.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out Vector3 angVel);
        _inputData._HMD.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 vel);
        debugMessages.Add("Device velocity: " + vel);
        Vector3 force = new Vector3(vel.x, 0, 0);
        debugMessages.Add("Non transformed " + force);
        debugMessages.Add("Transformed: " + transform.TransformVector(force));


        // Update crouche threshold
        _inputData._rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool isTriggerPressed);
        if (isTriggerPressed)
        {
            _inputData._HMD.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 currentPosition);
            crouchThreshold = currentPosition.y;
        }

        // Test();

        // Detech jump
        if (IsGrounded() && IsCrouching()) {
            jumpCharge += Time.deltaTime * 2f;
        } else {
            if (jumpCharge > minJumpCharge) {
                Jump();
            }
            jumpCharge = 0f;
        }

        debugScreen.text = string.Join("\n", debugMessages);
    }

    void UpdateColliderPosition()
    {
        Vector3 center = _xrOrigin.CameraInOriginSpacePos;
        _collider.center = new Vector3(center.x, _collider.center.y, center.z);
        _collider.height = _xrOrigin.CameraInOriginSpaceHeight;
    }
    bool IsGrounded()
    {
        return Physics.CheckSphere(transform.position, 0.5f, _groundLayer);
    }

    bool IsCrouching()
    {
        _inputData._HMD.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 pos);
        return pos.y <= crouchThreshold;
    }

    void Test()
    {
        _inputData._rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isTriggerPressed);
        if (isTriggerPressed)
        {
            List<XRNodeState> nodeStates = new List<XRNodeState>();
            InputTracking.GetNodeStates(nodeStates);

            foreach (var nodeState in nodeStates)
            {
                if (nodeState.nodeType == XRNode.Head && nodeState.TryGetVelocity(out Vector3 localVelocity))
                {
                    localVelocity.y = 0;
                    localVelocity.z = 0;
                    Vector3 worldVelocity = Camera.main.transform.TransformDirection(localVelocity);
                    // Debug.Log("World Velocity: " + worldVelocity);

                    _rb.AddForce(worldVelocity, ForceMode.Impulse);
                }
            }


            // _inputData._HMD.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 moveVect);
            // moveVect = Camera.main.transform.TransformDirection(moveVect);
            // moveVect *= 1.5f;
            // _rb.AddForce(moveVect, ForceMode.Impulse);
            // debugMessages.Add("Applied force: " + moveVect);
        }
    }

    void Jump()
    {
        _inputData._HMD.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 localVelocity);
        localVelocity.y = 0;
        localVelocity.z = 0;
        Vector3 headVelocity = Camera.main.transform.TransformDirection(localVelocity);

        jumpCharge = Math.Min(jumpCharge, maxJumpCharge);
        Vector3 force = Vector3.up * jumpCharge * 7f + (headVelocity * 3f);
        _rb.AddForce(force, ForceMode.Impulse);

    }
}
