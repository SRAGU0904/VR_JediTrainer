using System.Collections;
using UnityEngine;

public class WorkaroundFreezePhysics : MonoBehaviour {
	public float freezeTime;
	public Vector3 freezePosition;

	public static WorkaroundFreezePhysics Freeze(GameObject go, float freezeTime = float.Epsilon) {
		WorkaroundFreezePhysics result = go.AddComponent<WorkaroundFreezePhysics>();
		result.freezeTime = freezeTime;
		result.freezePosition = go.transform.position;
		return result;
	}
	
	IEnumerator Start() {
		float timeElapsed = 0;
		Rigidbody rb = GetComponent<Rigidbody>();
		Vector3 velBak = rb.linearVelocity;
		bool isKinematic = rb.isKinematic;
		bool useGravity = rb.useGravity;
		rb.isKinematic = true;
		rb.useGravity = false;
		while (timeElapsed < freezeTime) {
			transform.position = freezePosition;
			yield return new WaitForFixedUpdate();
			timeElapsed += Time.fixedDeltaTime;
		}
		rb.useGravity = useGravity;
		rb.isKinematic = isKinematic;
		if (!isKinematic) {
			rb.linearVelocity = velBak;
		}
		Destroy(this);
	}
}
