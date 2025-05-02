using UnityEngine;
using UnityEngine.XR;

// https://www.youtube.com/watch?v=GRSOrkmasMM
// https://www.youtube.com/watch?v=Mfim9MlgYWY
// https://www.youtube.com/watch?v=Xf2eDfLxcB8

[RequireComponent(typeof(InputData))]
[RequireComponent(typeof(PhysicalCharacterController))]
public class JumpDetector : MonoBehaviour {

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

    // Update is called once per frame
    void Update() {
        // Update crouch threshold
        _inputData._rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool isTriggerPressed);
        if (isTriggerPressed) {
            _inputData._HMD.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 currentPosition);
            crouchThreshold = currentPosition.y;
        }


        // Update crouch threshold
        _inputData._rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isTriggerPressed2);
        if (_physicalCharacterController.IsGrounded() && IsCrouching() && isTriggerPressed2) {
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
        _inputData._HMD.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 pos);
        return pos.y <= crouchThreshold;
    }

}
