using UnityEngine;

public class LookAtMe : MonoBehaviour
{
    void Update()
    {
        transform.rotation = ComputeRotation();
    }

	Vector3 TargetDirection() {
		return (transform.position - Camera.main.transform.position).normalized;
	}

    Quaternion ComputeRotation() {
        Quaternion rotation = Quaternion.LookRotation(TargetDirection());
        rotation.x = 0;
        rotation.z = 0;
        return rotation;
    }
}


