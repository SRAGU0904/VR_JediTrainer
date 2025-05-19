using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class JumpTutorial : MonoBehaviour {

    public GameObject player;
    public InputActionReference thresholdOkButton;
    public float firstJumpOffset = 1.5f;

    private HolocronV2 holocron;
    private Jump jump;
    private List<string> initialHolocronMessages;
    private float initialMinJumpCharge;
    private float initialMaxJumpCharge;

    private State state;

    private void OnEnable() {
        thresholdOkButton.action.Enable();

        Jump.OnThresholdSet += OnThresholdSet;
        thresholdOkButton.action.performed += OnThresholdOk;
        Jump.OnJumpReady += OnJumpReady;
        Jump.OnJump += OnJump;
    }

    private void OnDisable() {
        thresholdOkButton.action.Disable();

        Jump.OnThresholdSet -= OnThresholdSet;
        thresholdOkButton.action.performed -= OnThresholdOk;
        Jump.OnJumpReady -= OnJumpReady;
        Jump.OnJump -= OnJump;
    }


    void Start() {
        state = State.ThresholdNotDefined;

        holocron = GetComponent<HolocronV2>();
        initialHolocronMessages = new List<string>(holocron.messages);
        holocron.messages = new List<string> { initialHolocronMessages[0] };

        jump = player.GetComponent<Jump>();
        initialMinJumpCharge = jump.minJumpCharge;
        initialMaxJumpCharge = jump.maxJumpCharge;

        jump.minJumpCharge = float.MaxValue;
        jump.maxJumpCharge = float.MaxValue;
    }

    void Update() {
        if (state != State.ThresholdNotDefined) UpdateSecondMessage();
    }

    private void UpdateSecondMessage() {
        string newMessage = initialHolocronMessages[(int)State.TestingThreshold] + "\n";
        if (jump.IsCrouching()) {
            newMessage += "Currently:\nYou are crouching";
        }
        else {
            newMessage += "Currently:\nYou are not crouching";
        }
        holocron.messages[(int)State.TestingThreshold] = newMessage;
    }

    private void OnThresholdSet() {
        if (state == State.ThresholdNotDefined) {
            state = State.TestingThreshold;
            holocron.messages.Add(initialHolocronMessages[(int)state]);
            holocron.DisplayMessage((int)state);
        }
    }

    private void OnThresholdOk(InputAction.CallbackContext context) {
        if (context.performed) {
            State oldState = state;
            state = State.ThresholdOk;
            holocron.messages.Add(initialHolocronMessages[(int)state]);
            holocron.DisplayMessage((int)state);

            jump.minJumpCharge = initialMinJumpCharge + firstJumpOffset;
            jump.maxJumpCharge = initialMaxJumpCharge + firstJumpOffset;

            jump.jumpCharge = 0f;

            thresholdOkButton.action.performed -= OnThresholdOk;
            if (oldState != State.TestingThreshold) {
                OnJumpReady();
                OnJump();
                holocron.DisplayMessage(1);
            }
        }
    }

    private void OnJumpReady() {
        if (state == State.ThresholdOk) {
            state = State.JumpReady;
            holocron.messages.Add(initialHolocronMessages[(int)state]);
            holocron.DisplayMessage((int)state);

            Jump.OnJumpReady -= OnJumpReady;        
        }
    
    }

    private void OnJump() {
        state = State.TutorialFinished;
        holocron.messages.Add(initialHolocronMessages[(int)state]);
        holocron.DisplayMessage((int)state);

        jump.minJumpCharge = initialMinJumpCharge;
        jump.maxJumpCharge = initialMaxJumpCharge;
        Jump.OnJump -= OnJump;
        
    }

    private enum State {
        ThresholdNotDefined, TestingThreshold, ThresholdOk, JumpReady, TutorialFinished
    }
    
}
