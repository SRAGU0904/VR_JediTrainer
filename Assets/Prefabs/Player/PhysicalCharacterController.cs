using System;
using UnityEngine;

// https://docs.unity3d.com/ScriptReference/CharacterController.Move.html
public class PhysicalCharacterController : MonoBehaviour {
    public static event Action OnLand;

    private CharacterController controller;
    public Vector3 playerVelocity;

    public float zVelocity = 1f;

    public float gravityValue = -9.81f;
    private float defaultGravity;

    private bool landed = false;

    private void Start() {
        controller = gameObject.GetComponent<CharacterController>();
        defaultGravity = gravityValue;
        landed = false;
    }

    void Update() {
        if (IsGrounded() && playerVelocity.y < 0) {
            playerVelocity.y = 0f;
        }
        if (IsGrounded() && !landed) {
            OnLand?.Invoke();
        }
        landed = !IsGrounded();

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

    public void ResetGravity() {
        gravityValue = defaultGravity;
    }
}