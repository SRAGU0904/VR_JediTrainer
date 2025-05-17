using UnityEngine;
using TMPro;
using System.Collections;

public class UIInstructionTrigger : MonoBehaviour
{
	public TextMeshProUGUI textPanel;       // Reference to the text display
	public string updatedText = "Good job!\n\n Now go forward and try to defeat the enemies in this room! Press ? to skip the enermy";

	private bool alreadyTriggered = false;  // Ensures the trigger only happens once

	// Called when another collider enters this trigger
	private void OnTriggerEnter(Collider other)
	{
		// Only respond to lightsaber hits, and only once
		if (alreadyTriggered || !other.CompareTag("Lightsaber"))
			return;

		alreadyTriggered = true;

		// Update the on-screen instruction text
		textPanel.text = updatedText;
		textPanel.fontSize = 0.3f;

		Debug.Log("[INSTRUCTION] Holocron hit by lightsaber — text updated.");

		// Wait 3 seconds before notifying the player controller to advance
		StartCoroutine(DelayedClear());
	}

	// Waits for 3 seconds before advancing the player to the next path point
	private IEnumerator DelayedClear()
	{
		yield return new WaitForSeconds(5f);
		FindObjectOfType<PlayerPathController>()?.OnEnemyGroupCleared();
	}
}