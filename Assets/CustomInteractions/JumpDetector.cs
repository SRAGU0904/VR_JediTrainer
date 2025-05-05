using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

// https://www.youtube.com/watch?v=GRSOrkmasMM
// https://www.youtube.com/watch?v=Mfim9MlgYWY
// https://www.youtube.com/watch?v=Xf2eDfLxcB8

[RequireComponent(typeof(InputData))]
[RequireComponent(typeof(PhysicalCharacterController))]
public class JumpDetector : MonoBehaviour {

    public InputActionReference jumpButton;
    public InputActionReference setCrouchThresholdButton;
    public float crouchThreshold = 0.25f;
    public float maxJumpHeight = 3.0f;
    public float minJumpCharge = 0.25f;
    public float maxJumpCharge = 1f;

    private InputData _inputData;
    private PhysicalCharacterController _physicalCharacterController;
    private float jumpCharge = 0f;

    void Start() {
        _inputData = GetComponent<InputData>();
        _physicalCharacterController = GetComponent<PhysicalCharacterController>();
    }

    void OnEnable() {
        jumpButton.action.Enable();
        setCrouchThresholdButton.action.Enable();
        setCrouchThresholdButton.action.performed += SetCrouchThreshold;
    }

    void OnDisable() {
        jumpButton.action.Disable();
        setCrouchThresholdButton.action.Disable();
        setCrouchThresholdButton.action.performed -= SetCrouchThreshold;
    }

    private void SetCrouchThreshold(InputAction.CallbackContext context) {
        if (context.performed) {
            _inputData._HMD.TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition, out Vector3 currentPosition);
            crouchThreshold = currentPosition.y;            
        }
    }

    // Update is called once per frame
    void Update() {
        if (_physicalCharacterController.IsGrounded() && IsCrouching() && jumpButton.action.IsPressed()) {
            jumpCharge += Time.deltaTime;
        }
        else {
            if (jumpCharge > minJumpCharge) {
                float jumpHeight = maxJumpHeight * jumpCharge / maxJumpCharge;
                _physicalCharacterController.Jump(jumpHeight);
            }
            jumpCharge = 0f;
        }
    }

    bool IsCrouching() {
        _inputData._HMD.TryGetFeatureValue(UnityEngine.XR.CommonUsages.devicePosition, out Vector3 pos);
        return pos.y <= crouchThreshold;
    }

}
