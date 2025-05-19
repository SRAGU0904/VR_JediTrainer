using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HologramTutorial : MonoBehaviour {

    public List<string> tutorialMessages = new List<string>{"first message", "second message"};

    public HolocronV2 holocron;
    public Slider tutorialLeftRightSlider;
    public Slider tutorialRightLeftSlider;

    private int tutorialState;
    private List<string> initialHolocronMessages;

    void Start() {
        tutorialState = 0;

        tutorialRightLeftSlider.gameObject.SetActive(true);
        tutorialLeftRightSlider.gameObject.SetActive(false);

        holocron.leftRightSlider.gameObject.SetActive(false);
        holocron.rightLeftSlider.gameObject.SetActive(false);

        initialHolocronMessages = new List<string>(holocron.messages);
        holocron.messages = tutorialMessages;
        holocron.DisplayMessage(0);
    }


    public void OnMoveRight() {
        if (tutorialState != 1) return;
        tutorialState++;
        tutorialLeftRightSlider.gameObject.SetActive(false);
        EndTutorial();
    }

    public void OnMoveLeft() {
        if (tutorialState != 0) return;
        tutorialState++;
        tutorialRightLeftSlider.gameObject.SetActive(false);
        tutorialLeftRightSlider.gameObject.SetActive(true);
        holocron.DisplayMessage(1);
    }

    private void EndTutorial() {
        holocron.leftRightSlider.gameObject.SetActive(true);
        holocron.rightLeftSlider.gameObject.SetActive(true);
        holocron.messages = initialHolocronMessages;
        holocron.DisplayMessage(0);
    }
}
