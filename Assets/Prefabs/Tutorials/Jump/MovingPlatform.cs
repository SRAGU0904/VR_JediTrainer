using UnityEngine;
using UnityEngine.UIElements;

public class MovingPlatform : MonoBehaviour {
    private Vector3 pointA; // Starting point
    public Transform pointB; // Ending point
    public float speed = 2.0f; // Speed of the platform
    public bool isOn = true;
    public bool onlyOnce = false;

    private float threshold = 0.05f;

    private Vector3 target;

    void Start() {
        // Initialize the target to be point B
        pointA = transform.position;
        target = pointB.position;
    }

    void Update() {
        if (!isOn) return;

        // Move the platform towards the target
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // Check if the platform has reached the target within the threshold
        if (Vector3.Distance(transform.position, pointA) < threshold && target == pointA) {
            target = pointB.position;
            if (onlyOnce) isOn = false;
        }
        else if (Vector3.Distance(transform.position, pointB.position) < threshold && target == pointB.position) {
            target = pointA;
            if (onlyOnce) isOn = false;
        }
    }

    public void Toggle() {
        isOn = !isOn;
    }
}
