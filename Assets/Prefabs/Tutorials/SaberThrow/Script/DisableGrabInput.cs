using UnityEngine;
using UnityEngine.InputSystem;

public class DisableGrabInput : MonoBehaviour
{
	public InputActionReference leftGrabAction;
	public InputActionReference rightGrabAction;

	void Start()
	{
		if (leftGrabAction != null)
			leftGrabAction.action.Disable();

		if (rightGrabAction != null)
			rightGrabAction.action.Disable();
	}
}