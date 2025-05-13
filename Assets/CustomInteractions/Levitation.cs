using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PhysicalCharacterController))]
public class Levitation : MonoBehaviour {
    public InputActionReference levitationButton;
    public InputActionReference leftControllerRotationAction;
    public InputActionReference rightControllerRotationAction;
    public float levitationGravity = -2f;

    public float requiredControllerAngle = 80f;
    public float requiredControllerAngleThreshold = 15f;

    private PhysicalCharacterController _physicalCharacterController;

    private bool levitationEnabled = false;

    void Start() {
        _physicalCharacterController = GetComponent<PhysicalCharacterController>();
    }

    void OnEnable() {
        levitationButton.action.Enable();
        leftControllerRotationAction.action.Enable();
        rightControllerRotationAction.action.Enable();
    }

    void OnDisable() {
        levitationButton.action.Disable();
        leftControllerRotationAction.action.Disable();
        rightControllerRotationAction.action.Disable();
    }

    void Update() {
        bool levitationRequested = LevitationRequested();
        if (!levitationEnabled && levitationRequested) {
            _physicalCharacterController.gravityValue = levitationGravity;
            _physicalCharacterController.playerVelocity = new Vector3(_physicalCharacterController.playerVelocity.x, 0, _physicalCharacterController.playerVelocity.z);
            levitationEnabled = true;
        }
        if (levitationEnabled && !levitationRequested) {
            _physicalCharacterController.ResetGravity();
            levitationEnabled = false;
        }
    }

    private bool LevitationRequested() {
        float leftRotation = leftControllerRotationAction.action.ReadValue<Quaternion>().eulerAngles.x;
        float rightRotation = leftControllerRotationAction.action.ReadValue<Quaternion>().eulerAngles.x;

        bool goodLeftRotation = requiredControllerAngle - requiredControllerAngleThreshold < leftRotation && leftRotation < requiredControllerAngle + requiredControllerAngleThreshold;
        bool goodRightRotation = requiredControllerAngle - requiredControllerAngleThreshold < rightRotation && rightRotation < requiredControllerAngle + requiredControllerAngleThreshold;

        bool goodRotations = goodLeftRotation && goodRightRotation;

        return goodRotations && !_physicalCharacterController.IsGrounded() && _physicalCharacterController.playerVelocity.y < 0 && levitationButton.action.IsPressed();
    }
}