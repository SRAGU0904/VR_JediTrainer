using System;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class Tut3Counter : MonoBehaviour {
	public int numToComplete = 3;
	public int myStage = -1;
	public TMP_Text text;
	[TextArea(3, 10)]
	public string completedText = "Congrats!\nTutorial completed.\nTeleporting...";
	private int counter = 0;

	public void OnValidate() {
		Assert.IsTrue(numToComplete > 0, "Positive number of required successes!");
		Assert.IsTrue(myStage >= 0, "Stage index not set!");
		UpdateText();
	}

	private void UpdateText() {
		if (counter < numToComplete) {
			text.text =  $"{counter} / {numToComplete}";
		}
		else {
			text.text = completedText;
		}
	}
	
	public void Success() {
		counter++;
		Debug.Log($"Counter: {counter}");
		UpdateText();
		if (counter >= numToComplete) {
			TutorialController.StageFinished(myStage);
		}
	}
}
