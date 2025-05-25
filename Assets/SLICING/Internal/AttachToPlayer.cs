using UnityEngine;

public class AttachToPlayer : MonoBehaviour
{
	public Transform rightController;
	void Update()
    {
	    transform.position = rightController.position;
	    transform.rotation = Quaternion.Euler(-30, 0, 0) * rightController.rotation;
    }
}
