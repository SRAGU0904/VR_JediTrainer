using UnityEngine;

public class IsCrouchingText : MonoBehaviour
{

    public GameObject player;
    private HolocronV2 holocronV2;
    private Jump jump;
    private string initialMessage;

    void Start() {
        holocronV2 = GetComponent<HolocronV2>();
        initialMessage = holocronV2.messages[0];
        jump = player.GetComponent<Jump>();
    }

    void Update()
    {
        if (holocronV2.currentMessage == 0) {
            string newMessage = initialMessage + "\n\n";
            if (jump.IsCrouching()) {
                newMessage += "Currently:\nYou are crouching";
            }
            else {
                newMessage += "Currently:\nYou are not crouching";
            }
            holocronV2.messages[0] = newMessage;
            holocronV2.DisplayMessage(0);
        }
    }
}
