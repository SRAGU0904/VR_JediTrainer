using UnityEngine;

// https://docs.unity3d.com/ScriptReference/CharacterController.Move.html
public class PhysicalCharacterController : MonoBehaviour {
    private CharacterController controller;
    private Vector3 playerVelocity;

    public float zVelocity = 1f;

    private float gravityValue = -9.81f;

    private void Start() {
        controller = gameObject.GetComponent<CharacterController>();
    }

    void Update() {
        if (IsGrounded() && playerVelocity.y < 0) {
            playerVelocity.y = 0f;
        }

        // Apply gravity
        playerVelocity.y += gravityValue * Time.deltaTime;

        playerVelocity.z = zVelocity;

        // Combine horizontal and vertical movement
        Vector3 finalMove = playerVelocity.y * Vector3.up + playerVelocity.z * Vector3.forward;
        controller.Move(finalMove * Time.deltaTime);
    }

    public void Jump(float jumpHeight) {
        playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
    }

    public bool IsGrounded() {
        return controller.isGrounded;
    }
}