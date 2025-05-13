using UnityEngine;

public class Levitate : MonoBehaviour
{
    // Variables to control the levitation
    public float floatSpeed = 1.0f; // Speed of the floating movement
    public float floatHeight = 0.5f; // Height of the floating movement
    public float rotationSpeed = 30.0f; // Speed of the rotation

    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        // Store the initial position of the object
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the new Y position using a sine wave
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        // Update the object's position
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);

        // Rotate the object around its up axis
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
