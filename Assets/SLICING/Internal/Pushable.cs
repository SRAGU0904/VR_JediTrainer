using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Slicing {

	public class Pushable : MonoBehaviour {
		private float forceDecayPerSec;
		private float magnitudeZero;
		private Vector3 pushForce = Vector3.zero;
		private Rigidbody _rigidbody;
		private bool destroyOnStop;

		public static Pushable pushGameObject(GameObject go, Vector3 force, float initialPush = 0,
			float forceDecayPerSec = 0.5f, float magnitudeZero = 0.1f, bool destroyOnStop = true) {
			Rigidbody _rigidbody;
			if (!go.TryGetComponent(out _rigidbody)) {
				Debug.LogError("Tried pushing a GameObject without a Rigidbody!");
				return null;
			}

			Pushable pushable = go.GetOrAddComponent<Pushable>();
			pushable.forceDecayPerSec = forceDecayPerSec;
			pushable.magnitudeZero = magnitudeZero;
			pushable.destroyOnStop = destroyOnStop;
			pushable._rigidbody = _rigidbody;
			pushable.Push(force, initialPush);
			return pushable;
		}

		public void Push(Vector3 force, float initialPush = 0) {
			pushForce += force;
			_rigidbody.linearVelocity += force * initialPush;
		}

		private void Update() {
			if (_rigidbody is null) {
				_rigidbody = GetComponent<Rigidbody>();
			}

			_rigidbody.linearVelocity += pushForce * Time.deltaTime;
			if (pushForce != Vector3.zero) {
				Debug.Log(_rigidbody.linearVelocity.magnitude);
			}

			pushForce *= Mathf.Pow(forceDecayPerSec, Time.deltaTime);
			if (pushForce.magnitude < magnitudeZero) {
				pushForce = Vector3.zero;
			}

			if (destroyOnStop && pushForce == Vector3.zero) {
				Destroy(this);
			}
		}
	}

}