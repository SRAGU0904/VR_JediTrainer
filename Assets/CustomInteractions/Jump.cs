using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InputData))]
[RequireComponent(typeof(PhysicalCharacterController))]
public class Jump : MonoBehaviour {

    public static event Action OnJump;
    public static event Action OnJumpReady;
    public static event Action OnThresholdSet;

    public InputActionReference setCrouchThresholdButton;
    public uint hapticChannel = 0;
    public float hapticAmplitude = 1f;
    public float hapticDuration = 1f;
    public float crouchThreshold = -1f;
    public float jumpThreshold = 0.1f;
    public float maxJumpHeight = 3.0f;
    public float minJumpCharge = 0.25f;
    public float maxJumpCharge = 1f;

    private InputData _inputData;
    private PhysicalCharacterController _physicalCharacterController;
    public float jumpCharge;

    private bool jumpReadyCalled = false;
    private bool maxFeedbackSent = false;

    void Start() {
        _inputData = GetComponent<InputData>();
        _physicalCharacterController = GetComponent<PhysicalCharacterController>();
        jumpReadyCalled = false;
        jumpCharge = 0f;
        maxFeedbackSent = false;
    }

    void OnEnable() {
        setCrouchThresholdButton.action.Enable();
        setCrouchThresholdButton.action.performed += SetCrouchThreshold;
    }

    void OnDisable() {
        setCrouchThresholdButton.action.Disable();
        setCrouchThresholdButton.action.performed -= SetCrouchThreshold;
    }

    private void SetCrouchThreshold(InputAction.CallbackContext context) {
        if (context.performed) {
            _inputData._HMD.TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition, out Vector3 currentPosition);
            _inputData._leftController.SendHapticImpulse(hapticChannel, hapticAmplitude, hapticDuration);
            crouchThreshold = currentPosition.y;
            OnThresholdSet?.Invoke();
        }
    }

    void Update() {
        if (_physicalCharacterController.IsGrounded() && IsCrouching()) {
            jumpCharge += Time.deltaTime;
            if (jumpCharge >= minJumpCharge && !jumpReadyCalled) {
                OnJumpReady?.Invoke();
                jumpReadyCalled = true;
            }
            if (jumpCharge >= maxJumpCharge && !maxFeedbackSent) {
                _inputData._leftController.SendHapticImpulse(hapticChannel, hapticAmplitude, hapticDuration);
                _inputData._rightController.SendHapticImpulse(hapticChannel, hapticAmplitude, hapticDuration);
                maxFeedbackSent = true;
            }
        }
        else {
            if (headLevel() > crouchThreshold + jumpThreshold) {
                if (jumpCharge > minJumpCharge) {
                    jumpCharge = Math.Min(jumpCharge, maxJumpCharge);
                    float jumpHeight = maxJumpHeight * (jumpCharge - minJumpCharge) / maxJumpCharge;
                    _physicalCharacterController.Jump(jumpHeight);
                    OnJump?.Invoke();

                }
                jumpCharge = 0f;
                jumpReadyCalled = false;
                maxFeedbackSent = false;
            }

        }
    }

    private float headLevel() {
        _inputData._HMD.TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition, out Vector3 pos);
        return pos.y;
    }

    public bool IsCrouching() {
        return headLevel() <= crouchThreshold;
    }

}
