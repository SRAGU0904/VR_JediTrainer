using UnityEngine;

public class SpinObject : MonoBehaviour
{
    public float rotationSpeed = 2f; // Degrees per second

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
