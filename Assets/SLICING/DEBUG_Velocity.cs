using System;
using UnityEngine;

public class DEBUG_Velocity : MonoBehaviour {
	public float limitVelocity;
	private Vector3 lastPosition;
	private Rigidbody _rigidbody;

    // Update is called once per frame

    public static void LimitVelocity(GameObject go, float limit, Vector3 startPosition) {
	    DEBUG_Velocity instance = go.AddComponent<DEBUG_Velocity>();
	    instance.limitVelocity = limit;
	    instance.lastPosition = startPosition;
	    instance._rigidbody = go.GetComponent<Rigidbody>();
	    instance.transform.position = startPosition;
	    Debug.Log($"Setting starting position of {go.name} to {startPosition}");
    }
    
    void FixedUpdate() {
	    Vector3 displacement = transform.position - lastPosition;
	    displacement = Vector3.ClampMagnitude(displacement, limitVelocity * Time.fixedDeltaTime);
	    lastPosition += displacement;
	    transform.position = lastPosition;
	    _rigidbody.linearVelocity = Vector3.ClampMagnitude(_rigidbody.linearVelocity, limitVelocity);
    }
}
