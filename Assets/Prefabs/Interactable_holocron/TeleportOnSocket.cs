using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class TeleportOnSocket : MonoBehaviour {
	public XRSocketInteractor socket;

	private void OnEnable() {
		socket.selectEntered.AddListener(OnHolocronPlaced);
	}

	private void OnDisable() {
		socket.selectEntered.RemoveListener(OnHolocronPlaced);
	}

	private void OnHolocronPlaced(SelectEnterEventArgs args) {
		var action = args.interactableObject.transform.GetComponent<HolocroneTeleportInfo>();
		action?.TriggerTeleport();
	}
}