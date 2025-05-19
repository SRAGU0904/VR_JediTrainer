using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HolocronV2 : MonoBehaviour {

    public List<string> messages = new List<string>();
    public int currentMessage = -1;
    public bool setToInitialMessageOnStart = false;

    public Slider leftRightSlider;
    public Slider rightLeftSlider;

    public TextMeshProUGUI targetText;

    void Start() {
        if (setToInitialMessageOnStart) DisplayMessage(currentMessage);
	}

    void Update() {
        if (currentMessage < 0 || currentMessage >= messages.Count) return;
        var message = string.Join("\n", messages[currentMessage]) + "\n";
        if (currentMessage > 0) message += "←";
        if (currentMessage < messages.Count - 1) message += "→";
        targetText.text = message;
 	}

    // Use this method to display a message present in the messages list
    public void DisplayMessage(int messageId) {
        if (messageId < 0 || messageId >= messages.Count) return;
        currentMessage = messageId;
    }

    public void OnMoveRight() {
        if (leftRightSlider.value == 1) DisplayMessage(currentMessage - 1);
        leftRightSlider.value = 0;
        leftRightSlider.interactable = false;
        StartCoroutine(ReEnableAfterDelay(leftRightSlider));
    }

    public void OnMoveLeft() {
        if (rightLeftSlider.value == 0) DisplayMessage(currentMessage + 1);
        rightLeftSlider.value = 1;
        rightLeftSlider.interactable = false;
        StartCoroutine(ReEnableAfterDelay(rightLeftSlider));
    }

    IEnumerator ReEnableAfterDelay(Slider slider) {
        yield return new WaitForSeconds(1f); // Wait for 0.5 seconds
        slider.interactable = true;
    }
}