using System.Transactions;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class HolocronSpawner : MonoBehaviour {
	public enum SceneChoice {MainHub, Jump, Force, Cut, Throw};
	public SceneChoice sceneChoice = SceneChoice.MainHub;

	public GameObject holoPrefab;
	public XRSocketInteractor spawnerSocket;

	private GameObject currentHolo;

	public void Start() {
		ResetHolocrone();
	}

	public void ResetHolocrone() {
		if (currentHolo != null) {
			Destroy(currentHolo);
		}

		if (spawnerSocket.hasSelection) {
			var existing = spawnerSocket.GetOldestInteractableSelected().transform.gameObject;
			Destroy(existing);
		}

		currentHolo = Instantiate(holoPrefab, spawnerSocket.transform.position, spawnerSocket.transform.rotation);

		// Set up teleport action on the sphere
		HolocroneTeleportInfo teleportInfo = currentHolo.GetComponent<HolocroneTeleportInfo>();
		switch (sceneChoice) {
			case SceneChoice.Jump:
				teleportInfo.TeleportAction = SwapScenes.GotoJump; break;
			case SceneChoice.Force:
				teleportInfo.TeleportAction = SwapScenes.GotoForce; break;
			case SceneChoice.Cut:
				teleportInfo.TeleportAction = SwapScenes.GotoSaberCut; break;
			case SceneChoice.Throw:
				teleportInfo.TeleportAction = SwapScenes.GotoSaberThrow; break;
			default:
				teleportInfo.TeleportAction = SwapScenes.GotoMainHub; break;
		}

		//Helps it snap into socket
		Rigidbody rb = currentHolo.GetComponent<Rigidbody>();
		if (rb != null) {
			rb.linearVelocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
		}
	}

	public void OnButtonPressed(SelectEnterEventArgs args) {
		ResetHolocrone();
	}
}