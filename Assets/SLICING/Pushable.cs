using System;
using UnityEngine;

public class Pushable : MonoBehaviour {
	public float forceDecayPerSec = 0.5f;
	public float magnitudeZero = 0.1f;
	private Vector3 pushForce = Vector3.zero;
	private Rigidbody rigidbody;
	
	public void Push(Vector3 force, float initialPush = 0) {
		pushForce += force;
		rigidbody.linearVelocity += force * initialPush;
	}

	public void CopyTo (Pushable other) {
		other.forceDecayPerSec = forceDecayPerSec;
		other.magnitudeZero = magnitudeZero;
	}

	private void Start() {
		rigidbody = GetComponent<Rigidbody>();
	}
	
	private void Update() {
		rigidbody.linearVelocity += pushForce * Time.deltaTime;
		if (pushForce != Vector3.zero) {
			Debug.Log(rigidbody.linearVelocity.magnitude);
		}
		pushForce *= Mathf.Pow(forceDecayPerSec, Time.deltaTime);
		if (pushForce.magnitude < magnitudeZero) {
			pushForce = Vector3.zero;
		}
	}
	
}
