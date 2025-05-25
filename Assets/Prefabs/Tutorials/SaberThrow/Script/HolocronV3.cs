using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HolocronV3 : MonoBehaviour
{
	public List<string> messages = new List<string>();
	public int currentMessage = -1;
	public bool setToInitialMessageOnStart = false;
	public TextMeshProUGUI targetText;

	void Start()
	{
		if (setToInitialMessageOnStart)
			DisplayMessage(currentMessage);
	}

	// External call to display a message at a specific index
	public void DisplayMessage(int messageId)
	{
		if (messageId < 0 || messageId >= messages.Count) return;

		currentMessage = messageId;
		UpdateDisplay();
	}

	// Move to next message
	public void NextMessage()
	{
		if (currentMessage < messages.Count - 1)
		{
			DisplayMessage(currentMessage + 1);
		}
	}

	private void UpdateDisplay()
	{
		if (targetText != null && currentMessage >= 0 && currentMessage < messages.Count)
		{
			targetText.text = messages[currentMessage];
		}
	}
}