using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleButton : MonoBehaviour {

    public InputActionReference toggleAction;

    void OnEnable() {
        toggleAction.action.Enable();
        toggleAction.action.performed += OnButtonPressed;
    }

    void OnDisable() {
        toggleAction.action.Disable();
        toggleAction.action.performed -= OnButtonPressed;
    }

    private void OnButtonPressed(InputAction.CallbackContext context) {
        if (context.performed && GetComponentInParent<BoomerangWeapon>().IsHeld()) {
            GetComponent<Animator>().SetTrigger("Toggle");
        }
    }
}
